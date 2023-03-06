using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace AggieEnterpriseApi.Authentication;

public interface ITokenService
{
    Task<string> GetValidToken(string url, string key, string secret, string scope);
}

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;

    private const string TokenCacheKey = "AggieEnterpriseToken";

    public TokenService(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
    {
        _clientFactory = clientFactory;
        _memoryCache = memoryCache;
    }

    public async Task<string> GetValidToken(string tokenUrl, string key, string secret, string scope)
    {
        // check if we have a token in cache
        if (_memoryCache.TryGetValue(TokenCacheKey, out string token))
        {
            return token;
        }

        // if not, get a new one
        var httpClient = _clientFactory.CreateClient();
        
        // set auth with our consumer key and secret
        httpClient.DefaultRequestHeaders.Add("Authorization",
            "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}")));

        // setup the request content
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", scope)
        });

        // make the request
        var request = await httpClient.PostAsync(tokenUrl, content);

        // error if we didn't get a 200
        request.EnsureSuccessStatusCode();
    
        // get back json with our JWT in access_token
        var response = await request.Content.ReadFromJsonAsync<TokenResponse>();
        
        // error if we didn't get a valid response
        if (response?.access_token == null)
        {
            throw new Exception("Invalid response from Aggie Enterprise API. Access token was not returned.");
        }

        // cache the token for expired_in seconds (minus 60 seconds for safety)
        _memoryCache.Set(TokenCacheKey, response.access_token, TimeSpan.FromSeconds(response.expires_in - 60));

        // return the token
        return response.access_token;
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private class TokenResponse
    {
        public string? access_token { get; set; }
        public string? token_type { get; set; }
        public string? scope { get; set; }
        public int expires_in { get; set; }
        
        
    }
}