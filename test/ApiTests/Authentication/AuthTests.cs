using System.Threading.Tasks;
using AggieEnterpriseApi.Extensions;
using Shouldly;
using Xunit;

namespace ApiTests.Authentication;

public class AuthTests
{
    [Fact]
    public async Task CanGetClient()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get("", "",  "", "", "apitests");
        
        client.ShouldNotBeNull();
        
        var result = await client.DeptParents.ExecuteAsync("ACBS001");
        
        var data = result.ReadData();
        
        data.ShouldNotBeNull();
    }
}