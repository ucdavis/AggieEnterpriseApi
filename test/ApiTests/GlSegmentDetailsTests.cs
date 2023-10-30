using System;
using System.Linq;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
using ApiTests.Setup;
using Shouldly;
using StrawberryShake;
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

        var typeOfApprovers = data.ErpFinancialDepartment.Approvers.Select(a => a.ApproverType).Distinct().ToList();
        //data.ErpFinancialDepartment.Approvers.Count.ShouldBe(6);
        var FoApprovers = data.ErpFinancialDepartment.Approvers.Where(a => a.ApproverType == "Fiscal Officer Approver").ToList();
        FoApprovers.Count().ShouldBe(5);
        FoApprovers[0].EmailAddress.ShouldBe("bmceligot@ucdavis.edu");
        FoApprovers[0].FirstName.ShouldBe("Brian");
        FoApprovers[0].LastName.ShouldBe("Mceligot");
        FoApprovers[0].UserId.ShouldBe("mceligot");
        FoApprovers[1].EmailAddress.ShouldBe("satanguay@ucdavis.edu");
        FoApprovers[1].FirstName.ShouldBe("Shannon");
        FoApprovers[1].LastName.ShouldBe("Tanguay");
        FoApprovers[1].UserId.ShouldBe("tanguay");
    }

    [Fact]
    public async Task GetDisplayDetailsGl()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.DisplayDetailsGl.ExecuteAsync(
            segmentString: "3110-13U20-ADNO003-238533-44-000-0000000000-000000-0000-000000-000000", 
            project: "0000000000", 
            entity: "3110", 
            fund: "13U20", 
            dept: "ADNO003", 
            account: "238533", 
            purpose: "44", 
            program: "000", 
            activity: "000000",
            validateCVRs : true);

        var data = result.ReadData();
        data.ShouldNotBeNull();

    }

    [Fact]
    public async Task GetDisplayDetailsPpm()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.DisplayDetailsPpm.ExecuteAsync(
            projectNumber: "KL0733ATC1",
            projectNumberString: "KL0733ATC1",
            segmentString: "KL0733ATC1-TASK01-ADNO001-501090",
            taskNumber: "TASK01"
            );

        var data = result.ReadData();
        data.ShouldNotBeNull();

    }
}