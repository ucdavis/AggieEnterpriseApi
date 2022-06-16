using System.Threading.Tasks;
using AggieEnterpriseApi.Extensions;
using Xunit;

namespace ApiTests;

public class ChartValidationTests
{
    [Fact]
    public async Task ValidChartString()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, TestData.GraphQlApiToken);

        var result = await client.GlValidateChartstring.ExecuteAsync("3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000", false);

        var data = result.ReadData();

        Assert.True(data.GlValidateChartstring.ValidationResponse.Valid, "Response should be valid");
    }
}