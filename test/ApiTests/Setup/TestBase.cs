using Microsoft.Extensions.Configuration;

namespace ApiTests.Setup;

public class TestBase
{
    protected readonly string Token;
    protected readonly string GraphQlUrl;

    protected TestBase()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestBase>()
            .Build();

        Token = configuration.GetSection("Token").Value;
        GraphQlUrl = configuration.GetSection("GraphQlUrl").Value;
    }
}