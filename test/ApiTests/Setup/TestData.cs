namespace ApiTests.Setup;
using Microsoft.Extensions.Configuration;

public static class TestData
{

    public static string GraphQlUrl = "https://graphql-data-server-erp-poc.aws.ait.ucdavis.edu/graphql";

    public static string Token
    {
        get
        {
            var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Config>()
            .Build();

            return configuration.GetSection("Token").Value;

        }
    }

}