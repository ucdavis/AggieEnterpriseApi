using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class GlSearchTests : TestBase
{
    // entity
    // fund
    // department
    // purpose
    // account
    // project
    // program
    // activity
    
    [Fact]
    public async Task SearchEntity()
    {
        const string name = "campus";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpEntityFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpEntitySearch.ExecuteAsync(filter, "1311");
        
        var data = result.ReadData();

        data.ErpEntity.ShouldNotBeNull();
        data.ErpEntity.Name.ShouldContain(name);
        
        data.ErpEntitySearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchFund()
    {
        const string name = "Campuswide";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpFundFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpFundSearch.ExecuteAsync(filter, "09505");
        
        var data = result.ReadData();

        data.ErpFund.ShouldNotBeNull();
        data.ErpFund.Name.ShouldContain(name);
        
        data.ErpFundSearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchDepartment()
    {
        const string name = "practice";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpFinancialDepartmentFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpDepartmentSearch.ExecuteAsync(filter, "1009374");
        
        var data = result.ReadData();

        data.ErpFinancialDepartment.ShouldNotBeNull();
        data.ErpFinancialDepartment.Name.ShouldContain(name);
        
        data.ErpFinancialDepartmentSearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchPurpose()
    {
        const string name = "academic";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpPurposeFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpPurposeSearch.ExecuteAsync(filter, "1B");
        
        var data = result.ReadData();

        data.ErpPurpose.ShouldNotBeNull();
        data.ErpPurpose.Name.ShouldContain(name);
        
        data.ErpPurposeSearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchAccount()
    {
        const string name = "academic";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpAccountFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpAccountSearch.ExecuteAsync(filter, "400010");
        
        var data = result.ReadData();

        data.ErpAccount.ShouldNotBeNull();
        data.ErpAccount.Name.ShouldContain(name);
        
        data.ErpAccountSearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchProject()
    {
        const string name = "academic";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpProjectFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpProjectSearch.ExecuteAsync(filter, "AR0000000B");
        
        var data = result.ReadData();

        data.ErpProject.ShouldNotBeNull();
        data.ErpProject.Name.ShouldContain(name);
        
        data.ErpProjectSearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchProgram()
    {
        const string name = "academic";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpProgramFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpProgramSearch.ExecuteAsync(filter, "50B");
        
        var data = result.ReadData();

        data.ErpProgram.ShouldNotBeNull();
        data.ErpProgram.Name.ShouldContain(name);
        
        data.ErpProgramSearch.Data.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task SearchActivity()
    {
        const string name = "academic";
        
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new ErpActivityFilterInput() { Name = new StringFilterInput { Contains = name } };
        var result = await client.ErpActivitySearch.ExecuteAsync(filter, "202011");
        
        var data = result.ReadData();

        data.ErpActivity.ShouldNotBeNull();
        data.ErpActivity.Name.ShouldContain(name);
        
        data.ErpActivitySearch.Data.ShouldNotBeEmpty();
    }
}