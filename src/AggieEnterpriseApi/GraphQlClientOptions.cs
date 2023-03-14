namespace AggieEnterpriseApi;

public class GraphQlClientOptions
{
    public GraphQlClientOptions(string queryEndpoint, string tokenEndpoint, string key, string secret,
        string scope = "default")
    {
        QueryEndpoint = queryEndpoint;
        TokenEndpoint = tokenEndpoint;
        Key = key;
        Secret = secret;
        Scope = scope;
    }

    public string QueryEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string Key { get; set; }
    public string Secret { get; set; }
    public string Scope { get; set; }
}