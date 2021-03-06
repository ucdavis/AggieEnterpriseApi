using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;


namespace ApiTests;
public class ChartValidationTests : TestBase
{
    [Fact]
    public async Task ValidChartString()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.GlValidateChartstring.ExecuteAsync("3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000", false);

        var data = result.ReadData();
        data.GlValidateChartstring.ValidationResponse.Valid.ShouldBeTrue("Response should be valid");
        data.GlValidateChartstring.Segments.Entity.ShouldBe("3110");
        data.GlValidateChartstring.Segments.Fund.ShouldBe("13U20");
        data.GlValidateChartstring.Segments.Department.ShouldBe("ADNO003");
        data.GlValidateChartstring.Segments.Account.ShouldBe("410000");
        data.GlValidateChartstring.Segments.Purpose.ShouldBe("43");
        data.GlValidateChartstring.Segments.Project.ShouldBe("0000000000");
        data.GlValidateChartstring.Segments.Program.ShouldBe("000");
        data.GlValidateChartstring.Segments.Activity.ShouldBe("000000");
    }
    
    [Fact]
    public async Task ValidChartSegments()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var segments = new GlSegmentInput
        {
            Entity = "3110",
            Fund = "13U20",
            Department = "ADNO003",
            Account = "410000",
            Purpose = "43",
            Project = "0000000000",
            Program = "000",
            Activity = "000000",
            Flex1 = "000000",
            Flex2 = "000000"
        };
        var result = await client.GlValidateChartSegments.ExecuteAsync(segments, false);

        var data = result.ReadData();
        data.GlValidateChartSegments.ValidationResponse.Valid.ShouldBeTrue("Response should be valid");
        data.GlValidateChartSegments.CompleteChartstring.ShouldBe("3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000");
    }

    [Fact]
    public async Task ValidChartSegmentsWithoutFlex()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var segments = new GlSegmentInput
        {
            Entity = "3110",
            Fund = "13U20",
            Department = "ADNO003",
            Account = "410000",
            Purpose = "43",
            Project = "0000000000",
            Program = "000",
            Activity = "000000",
        };
        var result = await client.GlValidateChartSegments.ExecuteAsync(segments, false);

        var data = result.ReadData();
        data.GlValidateChartSegments.ValidationResponse.Valid.ShouldBeTrue("Response should be valid");
        data.GlValidateChartSegments.CompleteChartstring.ShouldBe("3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000");
    }

    [Fact]
    public async Task InValidChartStringFormat()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.GlValidateChartstring.ExecuteAsync("3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000", false);

        result.Errors.Count.ShouldBe(1);
        result.Errors[0].Message.ShouldBe("400: Bad Request");
        
        // TODO: API changed to put error in extension -- once api is more stable update this with expectations
        //result.Errors[0].Message.ShouldBe("Variable \"$segmentString\" got invalid value \"3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000\"; Expected type \"GlSegmentString\". Value is not a valid GL Segment String. (3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000)  Must match pattern: /^[0-9]{3}[0-9AB]-[0-9A-Z][0-9]{3}[0-9A-Z]-[0-9P][0-9]{5}[0-9A-F]-[0-9]{5}[0-9A-EX]-[0-9][0-9A-Z]-[0-9A-Z]{3}-[0-9A-Z]{10}-[0-9X]{5}[0-9AB]-0000-000000-000000$/");
    }

    [Fact]
    public async Task InvalidPpmSegments()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var ppmSegments = new PpmSegmentInput
        {
            Project = "SP20000018",
            Task = "000001",
            Organization = "9300052",
            ExpenditureType = "508210",
            Award = "1234567",
            FundingSource = "12345"
        };

        var result = await client.PpmSegmentsValidate.ExecuteAsync(ppmSegments, accountingDate: null);
        
        var data = result.ReadData();
        
        data.PpmSegmentsValidate.ValidationResponse.Valid.ShouldBeFalse("Response should be invalid");
    }
}