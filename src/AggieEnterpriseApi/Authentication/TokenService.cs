using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace AggieEnterpriseApi.Authentication;

public interface ITokenService
{
    Task<string> GetValidToken(GraphQlClientOptions options);
    void ClearTokenCache(GraphQlClientOptions options);
}

public class TokenService : ITokenService
{
    // set buffer time in seconds
    private const int BufferTime = 300;

    //Instantiate a Singleton of the Semaphore with a value of 1. This means that only 1 thread can be granted access at a time.
    private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

    private readonly IHttpClientFactory _clientFactory;
    private readonly IMemoryCache _memoryCache;

    private const string TokenCacheKey = "AggieEnterpriseToken";

    public TokenService(IHttpClientFactory clientFactory, IMemoryCache memoryCache)
    {
        _clientFactory = clientFactory;
        _memoryCache = memoryCache;
    }

    public void ClearTokenCache(GraphQlClientOptions options)
    {
        _memoryCache.Remove(GetTokenCacheKey(options.Scope));
    }

    public async Task<string> GetValidToken(GraphQlClientOptions options)
    {
        return await GetValidToken(options.TokenEndpoint, options.Key, options.Secret, options.Scope);
    }

    private async Task<string> GetValidToken(string tokenUrl, string key, string secret, string scope)
    {
        var scopedTokenCacheKey = GetTokenCacheKey(scope);

        // check if we have a token in cache
        if (_memoryCache.TryGetValue(scopedTokenCacheKey, out string token))
        {
            return token;
        }

        // if not, get a new token. Use a semaphore to ensure only 1 thread is getting a token at a time.
        await SemaphoreSlim.WaitAsync();

        // if we waited on the semaphore, check the cache again to see if another thread got a token while we were waiting
        if (_memoryCache.TryGetValue(scopedTokenCacheKey, out string token2))
        {
            // if one appeared while we were waiting, release the semaphore and return the token
            SemaphoreSlim.Release();
            return token2;
        }

        // nothing in the cache, go get a token
        try
        {
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

            // cache the token for expired_in seconds (minus a little for safety)
            _memoryCache.Set(scopedTokenCacheKey, response.access_token,
                TimeSpan.FromSeconds(response.expires_in - BufferTime));

            // return the token
            return response.access_token;
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    private string GetTokenCacheKey(string scope)
    {
        return $"{TokenCacheKey}_{scope}";
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