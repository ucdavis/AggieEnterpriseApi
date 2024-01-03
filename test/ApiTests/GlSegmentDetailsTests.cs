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
        FoApprovers.Count().ShouldBe(3);

        var appr = FoApprovers.Where(a => a.UserId == "mceligot").FirstOrDefault();
        appr.EmailAddress.ShouldBe("bmceligot@ucdavis.edu");
        appr.FirstName.ShouldBe("Brian");
        appr.LastName.ShouldBe("Mceligot");
        appr.UserId.ShouldBe("mceligot");

        FoApprovers.SingleOrDefault(a => a.UserId == "bjgregg").ShouldNotBeNull();
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
            taskNumber: "TASK01",
            expendCode: "501090",
            organization: "ADNO001"
            );

        var data = result.ReadData();
        data.ShouldNotBeNull();
    }

    [Fact]
    public async Task SearchPpmExpenditureType()
    {

            var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

            var filter = new PpmExpenditureTypeFilterInput() { Name = new StringFilterInput { Contains = "faculty" } };
            var result = await client.PpmExpenditureTypeSearch.ExecuteAsync(filter, "770000");

            var data = result.ReadData();

            Assert.NotNull(data);

            Assert.NotNull(data.PpmExpenditureTypeByCode);
            data.PpmExpenditureTypeByCode.Name.ShouldContain("Facilities and Equipment Maintenace Services");


    }
}