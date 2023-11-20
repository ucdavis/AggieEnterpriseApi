using System;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class DeptTests : TestBase
{
    [Fact]
    public async Task GetDept1()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.DeptParents.ExecuteAsync("ACBS001");
        
        var data = result.ReadData();
        
        data.ShouldNotBeNull();
        
        data.ErpFinancialDepartment.ShouldNotBeNull();
        data.ErpFinancialDepartment.Code.ShouldBe("ACBS001");
        data.ErpFinancialDepartment.Enabled.ShouldBeTrue();
        data.ErpFinancialDepartment.Parent.ShouldNotBeNull();


        DoesDeptRollUp.Dept(data.ErpFinancialDepartment , "AAES00C").ShouldBeTrue();
        DoesDeptRollUp.Dept(data.ErpFinancialDepartment, "AAES00B").ShouldBeFalse();
    }

    [Fact]
    public async Task GetDept2()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.DeptParents.ExecuteAsync("AAES00C");

        var data = result.ReadData();

        data.ShouldNotBeNull();

        data.ErpFinancialDepartment.ShouldNotBeNull();
        data.ErpFinancialDepartment.Code.ShouldBe("AAES00C");
        data.ErpFinancialDepartment.Enabled.ShouldBeTrue();
        data.ErpFinancialDepartment.Parent.ShouldNotBeNull();


        DoesDeptRollUp.Dept(data.ErpFinancialDepartment, "AAES00C").ShouldBeTrue();
        DoesDeptRollUp.Dept(data.ErpFinancialDepartment, "AAES00B").ShouldBeFalse();
    }


}