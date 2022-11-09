using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;


namespace ApiTests;
public class PpmValidationTests : TestBase
{
    /// <summary>
    /// Note, currently the data for this is only in AIT, not AIT-test
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task ValidateGlFundCodeForPpmProjectAndTask()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.PpmTaskByProjectNumberAndTaskNumber.ExecuteAsync("FPADNO1111", "HHMI01");

        var data = result.ReadData();
        data.PpmTaskByProjectNumberAndTaskNumber.ShouldNotBeNull();
        data.PpmTaskByProjectNumberAndTaskNumber.TaskNumber.ShouldBe("HHMI01");
        data.PpmTaskByProjectNumberAndTaskNumber.Name.ShouldBe("HHMI occupancy funding");
        data.PpmTaskByProjectNumberAndTaskNumber.EligibleForUse.ShouldBeTrue();
        data.PpmTaskByProjectNumberAndTaskNumber.Chargeable.ShouldBeTrue();
        data.PpmTaskByProjectNumberAndTaskNumber.GlPostingFundCode.ShouldBe("13U20");
        data.PpmTaskByProjectNumberAndTaskNumber.GlPostingPurposeCode.ShouldBe("40");
    }

    /// <summary>
    /// This project is currently only in ait-sit
    /// And it has a shelf life, but is works as of 2022 11 09
    /// </summary>
    /// <returns></returns>
    [Fact(Skip = "This project is currently only in ait-sit and is time sensitive. It will fail after december")]
    public async Task ValidatePpmString()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.PpmStringSegmentsValidate.ExecuteAsync("K302300049-TASK01-ADNO006-770000");

        var data = result.ReadData();
        data.PpmStringSegmentsValidate.ShouldNotBeNull();
        data.PpmStringSegmentsValidate.ValidationResponse.Valid.ShouldBeTrue();

        //DO segment check
        data.PpmStringSegmentsValidate.Segments.ShouldNotBeNull();
        data.PpmStringSegmentsValidate.Segments.Project.ShouldBe("K302300049");

        //Do warning check
        data.PpmStringSegmentsValidate.Warnings.ShouldNotBeNull();
        data.PpmStringSegmentsValidate.Warnings.Count.ShouldBe(1);
        data.PpmStringSegmentsValidate.Warnings[0].SegmentName.ShouldBe("Project");
        data.PpmStringSegmentsValidate.Warnings[0].Warning.ShouldEndWith("Please update or transactions to this chartstring will be rejected after that date."); 
    }


}