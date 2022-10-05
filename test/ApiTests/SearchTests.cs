using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
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
}