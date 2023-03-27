using Microsoft.Extensions.Configuration;

namespace ApiTests.Setup;

public class TestBase
{
    protected readonly string Token;
    protected readonly string GraphQlUrl;
    protected readonly string ConsumerKey;
    protected readonly string ConsumerSecret;
    protected readonly string TokenEndpoint;
    protected readonly string ScopeApp;
    protected readonly string ScopeEnv;

    protected TestBase()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestBase>()
            .Build();

        Token = configuration.GetSection("Token").Value;
        GraphQlUrl = configuration.GetSection("GraphQlUrl").Value;
        ConsumerKey = configuration.GetSection("ConsumerKey").Value;
        ConsumerSecret = configuration.GetSection("ConsumerSecret").Value;
        TokenEndpoint = configuration.GetSection("TokenEndpoint").Value;
        ScopeApp = configuration.GetSection("ScopeApp").Value;
        ScopeEnv = configuration.GetSection("ScopeEnv").Value;
    }
}