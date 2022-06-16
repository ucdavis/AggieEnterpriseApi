using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
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
    
    [Fact]
    public async Task ValidChartSegments()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, TestData.GraphQlApiToken);

        var segments = new GlSegmentInput
        {
            Entity = "3110",
            Fund = "72160",
            Department = "9300202",
            Account = "775000",
            Purpose = "85",
            Project = "0000000000",
            Program = "000",
            Activity = "000000",
            Flex1 = "000000",
            Flex2 = "000000"
        };
        var result = await client.GlValidateChartSegments.ExecuteAsync(segments, false);

        var data = result.ReadData();

        Assert.True(data.GlValidateChartSegments.ValidationResponse.Valid, "Response should be valid");
    }
}