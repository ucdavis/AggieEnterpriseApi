using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Authentication;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests;

// Offline tests only. This class does not inherit the secret-backed integration TestBase.
public class QueryRetryTests
{
    private const string Query = "query Read { ping }";

    [Theory]
    [InlineData(502)]
    [InlineData(503)]
    [InlineData(504)]
    public async Task RecoversAndReplaysBodyAndHeaders(int status)
    {
        var failedContent = new TrackingContent();
        var bodies = new List<string>();
        var messages = new List<HttpRequestMessage>();
        var transport = new Stub(async (request, attempt, _) =>
        {
            messages.Add(request);
            bodies.Add(await request.Content!.ReadAsStringAsync());
            Assert.Equal("Bearer test-only", request.Headers.Authorization!.ToString());
            return attempt == 1 ? new HttpResponseMessage((HttpStatusCode)status) { Content = failedContent }
                : new HttpResponseMessage(HttpStatusCode.OK);
        });
        var delays = new List<TimeSpan>();
        using var client = Client(transport, delays);
        using var request = Request(Query);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "test-only");
        using var response = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, transport.Calls);
        Assert.Equal(bodies[0], bodies[1]);
        Assert.NotSame(messages[0], messages[1]);
        Assert.True(failedContent.Disposed);
        Assert.InRange(Assert.Single(delays).TotalMilliseconds, 250, 500);
    }

    [Fact]
    public async Task ExhaustionReturnsFinalResponseWithTwoBoundedDelays()
    {
        var transport = Status(502);
        var delays = new List<TimeSpan>();
        using var client = Client(transport, delays);
        using var response = await client.SendAsync(Request(Query));
        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Equal(3, transport.Calls);
        Assert.Equal(2, delays.Count);
        Assert.InRange(delays[0].TotalMilliseconds, 250, 500);
        Assert.InRange(delays[1].TotalMilliseconds, 500, 1000);
    }

    [Theory]
    [InlineData("mutation Write { createJournal { id } }")]
    [InlineData("subscription Watch { ping }")]
    [InlineData("query Read { ping } mutation Write { createJournal { id } }")]
    [InlineData("query Read { ping } query Other { ping }")]
    [InlineData("not graphql")]
    public async Task DoesNotRetryOtherOrAmbiguousOperations(string document)
    {
        var transport = Status(502);
        using var client = Client(transport);
        using var response = await client.SendAsync(Request(document));
        Assert.Equal(1, transport.Calls);
    }

    [Theory]
    [InlineData("[]")]
    [InlineData("{}")]
    [InlineData("invalid json")]
    [InlineData("{\"query\": 123}")]
    [InlineData("{\"query\":\"query Read { ping }\",\"operationName\":\"Write\"}")]
    public async Task DoesNotRetryUnknownPayloads(string json)
    {
        var transport = Status(502);
        using var client = Client(transport);
        using var request = new HttpRequestMessage(HttpMethod.Post, "https://example.invalid/graphql")
        { Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json") };
        using var response = await client.SendAsync(request);
        Assert.Equal(1, transport.Calls);
    }

    [Theory]
    [InlineData(200)]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    [InlineData(429)]
    [InlineData(500)]
    public async Task OtherStatusesIncludingGraphQlErrorsAreNotRetried(int status)
    {
        var transport = new Stub((_, _, _) => Task.FromResult(new HttpResponseMessage((HttpStatusCode)status)
        { Content = new StringContent("{\"errors\":[{\"message\":\"validation failed\"}]}") }));
        using var client = Client(transport);
        using var response = await client.SendAsync(Request(Query));
        Assert.Equal(1, transport.Calls);
    }

    [Fact]
    public async Task NetworkExceptionsAreNotRetried()
    {
        var transport = new Stub((_, _, _) => throw new HttpRequestException("offline test"));
        using var client = Client(transport);
        await Assert.ThrowsAsync<HttpRequestException>(() => client.SendAsync(Request(Query)));
        Assert.Equal(1, transport.Calls);
    }

    [Fact]
    public async Task CancellationDuringBackoffDisposesResponseAndStops()
    {
        using var cancellation = new CancellationTokenSource();
        var content = new TrackingContent();
        var transport = new Stub((_, _, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadGateway)
        { Content = content }));
        using var client = new HttpClient(new QueryRetryHandler((_, token) =>
        {
            cancellation.Cancel();
            return Task.Delay(TimeSpan.FromSeconds(10), token);
        }) { InnerHandler = transport });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.SendAsync(Request(Query), cancellation.Token));
        Assert.Equal(1, transport.Calls);
        Assert.True(content.Disposed);
    }

    [Fact]
    public async Task CancellationDuringSendStopsWithoutRetry()
    {
        using var cancellation = new CancellationTokenSource();
        var transport = new Stub(async (_, _, token) =>
        {
            cancellation.Cancel();
            await Task.Delay(TimeSpan.FromSeconds(10), token);
            return new HttpResponseMessage(HttpStatusCode.BadGateway);
        });
        using var client = Client(transport);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.SendAsync(Request(Query), cancellation.Token));
        Assert.Equal(1, transport.Calls);
    }

    [Theory]
    [InlineData(2, 3)]
    [InlineData(30, 1)]
    public async Task HonorsShortRetryAfterAndStopsForLongRetryAfter(int seconds, int attempts)
    {
        var transport = new Stub((_, _, _) =>
        {
            var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            response.Headers.RetryAfter = new RetryConditionHeaderValue(TimeSpan.FromSeconds(seconds));
            return Task.FromResult(response);
        });
        var delays = new List<TimeSpan>();
        using var client = Client(transport, delays);
        using var response = await client.SendAsync(Request(Query));
        Assert.Equal(attempts, transport.Calls);
        Assert.All(delays, delay => Assert.Equal(TimeSpan.FromSeconds(seconds), delay));
    }

    [Fact]
    public void OptInHasSeparateCacheAndDefaultOverloadsKeepExistingBehavior()
    {
        const string url = "https://example.invalid/graphql";
        var normal = GraphQlClient.Get(url, "test-only");
        Assert.Same(normal, GraphQlClient.Get(url, "test-only", false));
        var retrying = GraphQlClient.Get(url, "test-only", true);
        Assert.NotSame(normal, retrying);
        Assert.Same(retrying, GraphQlClient.Get(url, "test-only", true));
        normal = GraphQlClient.Get(url, url, "test-key", "test-secret");
        Assert.Same(normal, GraphQlClient.Get(url, url, "test-key", "test-secret", "default", false));
        Assert.NotSame(normal, GraphQlClient.Get(url, url, "test-key", "test-secret", "default", true));
    }

    [Fact]
    public async Task AuthenticationRefreshDoesNotResetTransientBudget()
    {
        var unauthorized = new TrackingContent();
        var tokens = new Tokens();
        var transport = new Stub((request, attempt, _) =>
        {
            Assert.Equal(attempt < 3 ? "old" : "refreshed", request.Headers.Authorization!.Parameter);
            return Task.FromResult(new HttpResponseMessage(attempt == 2 ? HttpStatusCode.Unauthorized : HttpStatusCode.BadGateway)
            { Content = attempt == 2 ? unauthorized : new StringContent("failure") });
        });
        using var client = new HttpClient(new AuthenticationDelegatingHandler(tokens,
            new GraphQlClientOptions("https://example.invalid", "https://example.invalid/token", "test", "test", "test"))
        {
            InnerHandler = new QueryRetryHandler((_, _) => Task.CompletedTask) { InnerHandler = transport }
        });
        using var response = await client.SendAsync(Request(Query));
        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
        Assert.Equal(4, transport.Calls); // Initial send + two transient retries + one authentication refresh.
        Assert.Equal(1, tokens.Refreshes);
        Assert.True(unauthorized.Disposed);
    }

    [Fact]
    public async Task AllGeneratedDocumentsAreClassifiedCorrectly()
    {
        // Exercise Strawberry Shake's actual serializer and the repository's generated
        // query/mutation documents, rather than relying only on hand-written payloads.
        var queries = 0;
        var mutations = 0;
        foreach (var type in typeof(GraphQlClient).Assembly.GetTypes())
        {
            if (!typeof(StrawberryShake.IDocument).IsAssignableFrom(type) || type.IsInterface) continue;
            var document = (StrawberryShake.IDocument)type.GetProperty("Instance")!.GetValue(null)!;
            if (document.Kind == StrawberryShake.OperationKind.Query) queries++;
            if (document.Kind == StrawberryShake.OperationKind.Mutation) mutations++;
            var transport = Status(502);
            var connection = new StrawberryShake.Transport.Http.HttpConnection(() =>
            {
                var client = Client(transport);
                client.BaseAddress = new Uri("https://example.invalid/graphql");
                return client;
            });
            var text = System.Text.Encoding.UTF8.GetString(document.Body.ToArray());
            var operation = (HotChocolate.Language.OperationDefinitionNode)
                HotChocolate.Language.Utf8GraphQLParser.Parse(text).Definitions[0];
            var request = new StrawberryShake.OperationRequest(operation.Name!.Value, document);
            await foreach (var response in connection.ExecuteAsync(request)) { }
            Assert.Equal(document.Kind == StrawberryShake.OperationKind.Query ? 3 : 1, transport.Calls);
        }
        Assert.True(queries > 0);
        Assert.True(mutations >= 2);
    }

    private sealed class Tokens : ITokenService
    {
        public int Refreshes { get; private set; }
        public Task<string> GetValidToken(GraphQlClientOptions options) => Task.FromResult(Refreshes == 0 ? "old" : "refreshed");
        public void ClearTokenCache(GraphQlClientOptions options) => Refreshes++;
    }

    private static HttpRequestMessage Request(string document) => new(HttpMethod.Post, "https://example.invalid/graphql")
    {
        Content = new StringContent(JsonConvert.SerializeObject(new { query = document }),
            System.Text.Encoding.UTF8, "application/json")
    };

    private static HttpClient Client(Stub transport, List<TimeSpan>? delays = null) =>
        new(new QueryRetryHandler((delay, _) =>
        {
            delays?.Add(delay);
            return Task.CompletedTask;
        }) { InnerHandler = transport });

    private static Stub Status(int status) => new((_, _, _) =>
        Task.FromResult(new HttpResponseMessage((HttpStatusCode)status)));

    private sealed class Stub : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, int, CancellationToken, Task<HttpResponseMessage>> _send;
        public int Calls { get; private set; }
        public Stub(Func<HttpRequestMessage, int, CancellationToken, Task<HttpResponseMessage>> send) => _send = send;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken token) =>
            _send(request, ++Calls, token);
    }

    private sealed class TrackingContent : StringContent
    {
        public bool Disposed { get; private set; }
        public TrackingContent() : base("temporary failure") { }
        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}
