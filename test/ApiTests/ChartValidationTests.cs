using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;


namespace ApiTests;
public class ChartValidationTests
{

    
    [Fact]
    public async Task ValidChartString()
    {
                var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, TestData.Token);

        var result = await client.GlValidateChartstring.ExecuteAsync("3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000", false);

        var data = result.ReadData();
        data.GlValidateChartstring.ValidationResponse.Valid.ShouldBeTrue("Response should be valid");
        data.GlValidateChartstring.Segments.Entity.ShouldBe("3110");
        data.GlValidateChartstring.Segments.Fund.ShouldBe("72160");
        data.GlValidateChartstring.Segments.Department.ShouldBe("9300202");
        data.GlValidateChartstring.Segments.Account.ShouldBe("775000");
        data.GlValidateChartstring.Segments.Purpose.ShouldBe("85");
        data.GlValidateChartstring.Segments.Project.ShouldBe("0000000000");
        data.GlValidateChartstring.Segments.Program.ShouldBe("000");
        data.GlValidateChartstring.Segments.Activity.ShouldBe("000000");
    }
    
    [Fact]
    public async Task ValidChartSegments()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, TestData.Token);

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
        data.GlValidateChartSegments.ValidationResponse.Valid.ShouldBeTrue("Response should be valid");
        data.GlValidateChartSegments.CompleteChartstring.ShouldBe("3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000");
    }

    [Fact]
    public async Task ValidChartSegmentsWithoutFlex()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, TestData.Token);

        var segments = new GlSegmentInput
        {
            Entity = "3110",
            Fund = "72160",
            Department = "9300202",
            Account = "775000",
            Purpose = "85",
            Project = "0000000000",
            Program = "000",
            Activity = "000000"
        };
        var result = await client.GlValidateChartSegments.ExecuteAsync(segments, false);

        var data = result.ReadData();
        data.GlValidateChartSegments.ValidationResponse.Valid.ShouldBeTrue("Response should be valid");
        data.GlValidateChartSegments.CompleteChartstring.ShouldBe("3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000");
    }

    [Fact]
    public async Task InValidChartStringFormat()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, TestData.Token);

        var result = await client.GlValidateChartstring.ExecuteAsync("3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000", false);

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe("BAD_USER_INPUT");
        result.Errors[0].Message.ShouldBe("Variable \"$segmentString\" got invalid value \"3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000\"; Expected type \"GlSegmentString\". Value is not a valid GL Segment String. (3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000)  Must match pattern: /^[0-9]{3}[0-9AB]-[0-9A-Z][0-9]{3}[0-9A-Z]-[0-9P][0-9]{5}[0-9A-F]-[0-9]{5}[0-9A-EX]-[0-9][0-9A-Z]-[0-9A-Z]{3}-[0-9A-Z]{10}-[0-9X]{5}[0-9AB]-0000-000000-000000$/");
    }
}