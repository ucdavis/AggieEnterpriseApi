using Microsoft.Extensions.Configuration;

namespace ApiTests.Setup;

public class TestBase
{
    protected readonly string Token;
    protected readonly string GraphQlUrl = "https://graphql-data-server-erp-poc.aws.ait.ucdavis.edu/graphql";

    protected TestBase()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TestBase>()
            .Build();

        Token = configuration.GetSection("Token").Value;
    }
}