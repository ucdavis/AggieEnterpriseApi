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
    
    
}