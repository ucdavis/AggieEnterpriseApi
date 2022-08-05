using System;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
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
        data.ErpFund.Parent.Parent.ShouldNotBeNull();
        data.ErpFund.Parent.Parent.Code.ShouldBe("1200C");


        DoesFundRollUp.Fund(data.ErpFund, 2, "1200C").ShouldBeTrue();
        DoesFundRollUp.Fund(data.ErpFund, 1, "1200C").ShouldBeFalse();
    }


}