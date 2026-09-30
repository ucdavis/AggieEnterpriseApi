using System.Net;
using HotChocolate.Language;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

namespace AggieEnterpriseApi;

// Registered inside AuthenticationDelegatingHandler. The per-request budget survives
// its one token refresh, so authentication does not restart the transient retry budget.
internal sealed class QueryRetryHandler : DelegatingHandler
{
    private const string BudgetKey = "AggieEnterpriseApi.QueryRetryBudget";
    private readonly Func<TimeSpan, CancellationToken, Task> _delay;
    private readonly Random _random = new();

    internal QueryRetryHandler() : this(Task.Delay) { }

    internal QueryRetryHandler(Func<TimeSpan, CancellationToken, Task> delay) => _delay = delay;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (!await IsQueryAsync(request).ConfigureAwait(false))
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (!request.Properties.TryGetValue(BudgetKey, out var value))
            request.Properties[BudgetKey] = value = new RetryBudget();
        var budget = (RetryBudget)value;
        var body = await request.Content!.ReadAsByteArrayAsync().ConfigureAwait(false);

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var attempt = CopyRequest(request, body);
            var response = await base.SendAsync(attempt, cancellationToken).ConfigureAwait(false);
            if (!IsTransient(response.StatusCode) || budget.Retries == 2)
            {
                if (budget.Retries > 0)
                    Log.Information("GraphQL query completed after {RetryCount} transient retries with HTTP {StatusCode}",
                        budget.Retries, (int)response.StatusCode);
                return response;
            }

            double jitter;
            lock (_random) jitter = _random.NextDouble();
            var delay = TimeSpan.FromMilliseconds(250 * (1 << budget.Retries) * (1 + jitter));
            var retryAfter = response.Headers.RetryAfter;
            var requestedDelay = retryAfter?.Delta ?? (retryAfter?.Date - DateTimeOffset.UtcNow);
            // Do not retry sooner than the server asks, or hold a caller for a long backoff.
            if (requestedDelay > TimeSpan.FromSeconds(5)) return response;
            if (requestedDelay > delay) delay = requestedDelay.Value;

            budget.Retries++;
            Log.Information("Retrying GraphQL query after HTTP {StatusCode}; retry {RetryCount} in {DelayMs} ms",
                (int)response.StatusCode, budget.Retries, delay.TotalMilliseconds);
            response.Dispose();
            await _delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }

    private static bool IsTransient(HttpStatusCode status) =>
        status == HttpStatusCode.BadGateway || status == HttpStatusCode.ServiceUnavailable ||
        status == HttpStatusCode.GatewayTimeout;

    private static async Task<bool> IsQueryAsync(HttpRequestMessage request)
    {
        if (request.Method != HttpMethod.Post ||
            request.Content?.Headers.ContentType?.MediaType != "application/json") return false;

        try
        {
            var payload = JObject.Parse(await request.Content.ReadAsStringAsync().ConfigureAwait(false));
            if (payload["query"]?.Type != JTokenType.String) return false;
            var document = Utf8GraphQLParser.Parse(payload["query"]!.Value<string>()!);
            var operations = document.Definitions.OfType<OperationDefinitionNode>().ToArray();
            if (operations.Length != 1 || operations[0].Operation != OperationType.Query) return false;
            var name = payload["operationName"];
            return name == null || name.Type == JTokenType.Null ||
                (name.Type == JTokenType.String && name.Value<string>() == operations[0].Name?.Value);
        }
        catch (JsonException) { return false; }
        catch (SyntaxException) { return false; }
    }

    private static HttpRequestMessage CopyRequest(HttpRequestMessage source, byte[] body)
    {
        var copy = new HttpRequestMessage(source.Method, source.RequestUri)
        {
            Version = source.Version,
            Content = new ByteArrayContent(body)
        };
        foreach (var header in source.Headers) copy.Headers.TryAddWithoutValidation(header.Key, header.Value);
        foreach (var header in source.Content!.Headers) copy.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        foreach (var property in source.Properties) copy.Properties[property.Key] = property.Value;
        return copy;
    }

    private sealed class RetryBudget
    {
        public int Retries;
    }
}
