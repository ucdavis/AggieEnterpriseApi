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
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var filter = new PpmProjectFilterInput { Name = new StringFilterInput { Contains = "annual" } };
        var result = await client.PpmProjectSearch.ExecuteAsync(filter, "KP0939277U");
        
        var data = result.ReadData();
        
        Assert.NotNull(data);

        Assert.NotNull(data.PpmProjectByNumber);
        Assert.Contains("TRAILER ALT", data.PpmProjectByNumber?.Name ?? string.Empty);
        
        Assert.NotEmpty(data.PpmProjectSearch.Data);
    }
}