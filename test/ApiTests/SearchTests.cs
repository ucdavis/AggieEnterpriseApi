using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class SearchTests : TestBase
{
    [Fact]
    public async Task SearchPpmProject()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new PpmProjectFilterInput { Name = new StringFilterInput { Contains = "annual" } };
        var result = await client.PpmProjectSearch.ExecuteAsync(filter, "KP0939277U");
        
        var data = result.ReadData();
        
        Assert.NotNull(data);

        Assert.NotNull(data.PpmProjectByNumber);
        Assert.Contains("TRAILER ALT", data.PpmProjectByNumber?.Name ?? string.Empty);
        
        Assert.NotEmpty(data.PpmProjectSearch.Data);
    }
    
    [Fact]
    public async Task FindPpmProjectWithTasks()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new PpmProjectFilterInput { Name = new StringFilterInput { Contains = "annual" } };
        var result = await client.PpmProjectWithTasks.ExecuteAsync("K309872537");
        
        var data = result.ReadData();

        Assert.NotNull(data);
        Assert.NotNull(data.PpmProjectByNumber);
        Assert.Contains("Circilla", data.PpmProjectByNumber?.Name ?? string.Empty);
        
        data.PpmProjectByNumber?.Tasks.ShouldNotBeEmpty();
    }
    
    [Fact]
    public async Task FindPpmSegmentNames()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.PpmSegmentsToNames.ExecuteAsync("K30APSD227", "K30APSD227", "TASK01", "APLS002", "770000", "000000", "000000");
        
        // var data = result.ReadData();
        var data = result.Data; // can't call read data because API currently errors erroneously

        data.ShouldNotBeNull();
        data.PpmProjectByNumber?.Name.ShouldContain("LUNDY");
        data.PpmTaskByProjectNumberAndTaskNumber?.Name.ShouldBe("TASK01");
        data.PpmOrganization?.Name.ShouldBe("APLS002 - PLS Faculty Resources");
        data.PpmExpenditureTypeByCode?.Name.ShouldBe("770000 - Facilities And Equipment Maintenace Services");
    }
    
    [Fact]
    public async Task SearchPpmOrg()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new PpmOrganizationFilterInput() { Name = new StringFilterInput { Contains = "faculty" } };
        var result = await client.PpmOrganizationSearch.ExecuteAsync(filter, "APLS002");
        
        var data = result.ReadData();
        
        Assert.NotNull(data);
        
        Assert.NotNull(data.PpmOrganization);
        Assert.Contains("PLS Faculty Resources", data.PpmOrganization?.Name ?? string.Empty);
        
        Assert.NotEmpty(data.PpmOrganizationSearch.Data);
    }
    
    [Fact]
    public async Task SearchPpmExpenditureType()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new PpmExpenditureTypeFilterInput() { Name = new StringFilterInput { Contains = "faculty" } };
        var result = await client.PpmExpenditureTypeSearch.ExecuteAsync(filter, "770000");
        
        var data = result.ReadData();
        
        Assert.NotNull(data);
        
        Assert.NotNull(data.PpmExpenditureTypeByCode);
        Assert.Contains("Facilities And Equipment Maintenace Services", data.PpmExpenditureTypeByCode?.Name ?? string.Empty);
        
        Assert.NotEmpty(data.PpmExpenditureTypeSearch.Data);
    }
    
    [Fact]
    public async Task SearchPpmTask()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new PpmTaskFilterInput() { Name = new StringFilterInput { Contains = "TASK" }, TaskNumber = new StringFilterInput() { Contains = "TASK01" }, ProjectId = new StringFilterInput() { Eq = "300000008444977"}};
        var result = await client.PpmTaskSearch.ExecuteAsync(filter);
        
        var data = result.ReadData();
        
        Assert.NotNull(data);
        
        Assert.NotNull(data.PpmTaskSearch);
        Assert.Contains("TASK01", data.PpmTaskSearch.Data[0].Name ?? string.Empty);
        
    }
}