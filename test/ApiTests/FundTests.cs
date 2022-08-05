using System;
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
        data.ErpFund.Parent.Parent.ShouldNotBeNull();
        data.ErpFund.Parent.Parent.Code.ShouldBe("1200C");

        DoesItRollUp(data.ErpFund, 2, "1200C").ShouldBeTrue();
        DoesItRollUp(data.ErpFund, 1, "1200C").ShouldBeFalse();
    }

    private bool DoesItRollUp(IFundParents_ErpFund? fund, int level, string expectedCode)
    {
        if (fund == null)
        {
            return false;
        }
        if (fund.HierarchyDepth == level && fund.Code == expectedCode)
        {
            return true;
        }
        if (fund.Parent == null)
        {
            return false;
        }
        
        if (fund.Parent.HierarchyDepth == level && fund.Parent.Code == expectedCode)
        {
            return true;
        }
        if (fund.Parent.Parent == null)
        {
            return false;
        }

        if (fund.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Code == expectedCode)
        {
            return true;
        }
        if (fund.Parent.Parent.Parent == null)
        {
            return false;
        }

        if (fund.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Code == expectedCode)
        {
            return true;
        }
        if (fund.Parent.Parent.Parent.Parent == null)
        {
            return false;
        }

        if (fund.Parent.Parent.Parent.Parent.HierarchyDepth == level && fund.Parent.Parent.Parent.Parent.Code == expectedCode)
        {
            return true;
        }
        if (fund.Parent.Parent.Parent.Parent.Parent == null)
        {
            return false;
        }


        return false;
    }

}