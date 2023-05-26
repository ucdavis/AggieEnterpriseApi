using System;
using System.Linq;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class GlSegmentDetailsTests : TestBase
{
    [Fact]
    public async Task GetDetails()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.CoaDetails.ExecuteAsync(entity: "3110", fund: "12100", dept: "ADNO003", account: "775000", purpose:"45");
        
        var data = result.ReadData();
        
        data.ShouldNotBeNull();

        data.ErpEntity.ShouldNotBeNull();
        data.ErpEntity.Code.ShouldBe("3110");
        data.ErpEntity.Name.ShouldBe("UC Davis Campus");

        data.ErpFund.ShouldNotBeNull();        
        data.ErpFund.Code.ShouldBe("12100");
        data.ErpFund.Name.ShouldBe("Sales and Services Rate Based");

        data.ErpFinancialDepartment.ShouldNotBeNull();
        data.ErpFinancialDepartment.Code.ShouldBe("ADNO003");
        data.ErpFinancialDepartment.Name.ShouldBe("ADNO Deans Office Admin");

        data.ErpAccount.ShouldNotBeNull();
        data.ErpAccount.Code.ShouldBe("775000");
        data.ErpAccount.Name.ShouldBe("UCD Recharge Revenue");

        data.ErpPurpose.ShouldNotBeNull();
        data.ErpPurpose.Code.ShouldBe("45");
        data.ErpPurpose.Name.ShouldBe("AES Research");

    }

    [Fact]
    public async Task GetDepartmentApprovers()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.ErpDepartmentApprovers.ExecuteAsync("ADNO003");

        var data = result.ReadData();
        data.ShouldNotBeNull();
        data.ErpFinancialDepartment.ShouldNotBeNull();
        data.ErpFinancialDepartment.Code.ShouldBe("ADNO003");
        data.ErpFinancialDepartment.Approvers.ShouldNotBeNull();
        data.ErpFinancialDepartment.Approvers.Count.ShouldBe(6);
        var FoApprovers = data.ErpFinancialDepartment.Approvers.Where(a => a.ApproverType == "Fiscal Officer Approver").ToList();
        FoApprovers.Count().ShouldBe(2);
        FoApprovers[0].EmailAddress.ShouldBe("bmceligot@ucdavis.edu");
        FoApprovers[0].FirstName.ShouldBe("Brian");
        FoApprovers[0].LastName.ShouldBe("Mceligot");
        FoApprovers[0].UserId.ShouldBe("mceligot");
        FoApprovers[1].EmailAddress.ShouldBe("satanguay@ucdavis.edu");
        FoApprovers[1].FirstName.ShouldBe("Shannon");
        FoApprovers[1].LastName.ShouldBe("Tanguay");
        FoApprovers[1].UserId.ShouldBe("tanguay");
    }
}