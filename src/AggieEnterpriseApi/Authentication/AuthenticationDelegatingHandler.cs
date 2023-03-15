using System.Net;
using System.Net.Http.Headers;

namespace AggieEnterpriseApi.Authentication;

/// <summary>
/// On any http call, get the bearer token from the token service and add it to the request.
/// If unauthorized, get a new token and try again once.
/// </summary>
public class AuthenticationDelegatingHandler : DelegatingHandler
{
    private const string BearerScheme = "Bearer";

    private readonly ITokenService _tokenService;
    private readonly GraphQlClientOptions _options;

    public AuthenticationDelegatingHandler(ITokenService tokenService, GraphQlClientOptions options)
    {
        _tokenService = tokenService;
        _options = options;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenService.GetValidToken(_options);

        request.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
        {
            // if we get unauthorized, clear the token cache and try again
            _tokenService.ClearTokenCache();

            token = await _tokenService.GetValidToken(_options);

            request.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, token);

            response = await base.SendAsync(request, cancellationToken);
        }

        return response;
    }
}