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
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

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

    [Fact]
    public async Task GetFundDetails()
    {
        const string fundCode = "36327";
        const string expectedFundPurpose = "Scholarships for such worthy students at the College of Agriculture located at Davis California";

        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.FundDetails.ExecuteAsync(fundCode);

        var data = result.ReadData();

        data.ShouldNotBeNull();

        data.ErpFund.ShouldNotBeNull();
        data.ErpFund.Code.ShouldBe(fundCode);
        data.ErpFund.FundPurpose.ShouldBe(expectedFundPurpose);
        data.ErpFund.GiftFund.ShouldBeFalse();
        data.ErpFund.EndowmentGiftFund.ShouldBeTrue();
    }
}
