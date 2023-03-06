using System.Net.Http.Json;
using System.Text;

namespace AggieEnterpriseApi.Authentication;

public class TokenService
{
    private readonly IHttpClientFactory _clientFactory;

    public TokenService(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public async Task<string> GetValidToken(string url, string key, string secret, string scope)
    {
        var httpClient = _clientFactory.CreateClient();

        // baseUrl is just host + port
        var baseUrl = new Uri(url).GetLeftPart(UriPartial.Authority);

        // set auth with our consumer key and secret
        httpClient.DefaultRequestHeaders.Add("Authorization",
            "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{key}:{secret}")));

        // set the content type
        httpClient.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");

        // set the scope
        var tokenUrl = $"{baseUrl}/oauth2/token";

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

        // return the token
        return response.access_token;
    }

    private class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
    }
}