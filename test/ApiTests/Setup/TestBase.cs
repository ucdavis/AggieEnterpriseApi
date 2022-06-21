using Microsoft.Extensions.Configuration;

namespace ApiTests.Setup;

public class TestBase
{
    protected readonly string Token;

    protected TestBase()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<ChartValidationTests>()
            .Build();

        Token = configuration.GetSection("Token").Value;
    }
}