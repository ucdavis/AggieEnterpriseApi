using System;
using System.Threading.Tasks;
using AggieEnterpriseApi.Extensions;
using Xunit;
using ApiTests.Setup;
using Shouldly;

namespace ApiTests;

public class PpmTeamTests : TestBase
{
    [Fact]
    public async Task? GetProjectsForPerson(){
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.PpmProjectByProjectTeamMemberEmployeeId.ExecuteAsync("10727488", null);

        var data = result.ReadData();
        
        data.PpmProjectByProjectTeamMemberEmployeeId.ShouldNotBeEmpty();

    }
}