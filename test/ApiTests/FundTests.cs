using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class FundTests : TestBase
{
    [Fact]
    public async Task GetFund()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.FundParents.ExecuteAsync("12100");
        
        var data = result.ReadData();
        
        data.ShouldNotBeNull();
        
        data.ErpFund.ShouldNotBeNull();
        data.ErpFund.Parent.ShouldNotBeNull();
        data.ErpFund.Parent.Code.ShouldBe("1210D");

    }
}