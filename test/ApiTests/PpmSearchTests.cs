using System.Linq;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class PpmSearchTests : TestBase
{
    [Fact]
    public async Task SearchPpmProject()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var filter = new PpmProjectFilterInput { Name = new StringFilterInput { Contains = "faculty" } };
        var result = await client.PpmProjectSearch.ExecuteAsync(filter, "K30GEAFAFU");

        var data = result.ReadData();

        Assert.NotNull(data);

        Assert.NotNull(data.PpmProjectByNumber);
        Assert.Contains("FACULTY", data.PpmProjectByNumber?.Name ?? string.Empty);

        Assert.NotEmpty(data.PpmProjectSearch.Data);
    }

    [Fact]
    public async Task FindPpmProjectWithTasks()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

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
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.PpmSegmentsToNames.ExecuteAsync("K30APSD227", "K30APSD227", "TASK01", "APLS002", "770000", "000000", "000000");

        // var data = result.ReadData();
        var data = result.Data; // can't call read data because API currently errors erroneously

        data.ShouldNotBeNull();
        data.PpmProjectByNumber?.Name.ShouldContain("LUNDY");
        data.PpmTaskByProjectNumberAndTaskNumber?.Name.ShouldBe("TASK01");
        data.PpmOrganization?.Name.ShouldBe("APLS002 - APLS Faculty Resources");
        data.PpmExpenditureTypeByCode?.Name.ShouldBe("770000 - Facilities and Equipment Maintenace Services");
    }

    [Fact]
    public async Task SearchPpmOrg()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

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
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var filter = new PpmExpenditureTypeFilterInput() { Name = new StringFilterInput { Contains = "faculty" } };
        var result = await client.PpmExpenditureTypeSearch.ExecuteAsync(filter, "770000");

        var data = result.ReadData();

        Assert.NotNull(data);

        Assert.NotNull(data.PpmExpenditureTypeByCode);
        Assert.Contains("Facilities and Equipment Maintenace Services", data.PpmExpenditureTypeByCode?.Name ?? string.Empty);

        Assert.NotEmpty(data.PpmExpenditureTypeSearch.Data);
    }

    [Fact]
    public async Task SearchPpmTask()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var filter = new PpmTaskFilterInput() { Name = new StringFilterInput { Contains = "TASK" }, TaskNumber = new StringFilterInput() { Contains = "TASK01" }, ProjectId = new StringFilterInput() { Eq = "300000014411052" } };
        var result = await client.PpmTaskSearch.ExecuteAsync(filter);

        var data = result.ReadData();

        Assert.NotNull(data);

        Assert.NotNull(data.PpmTaskSearch);
        Assert.Contains("TASK01", data.PpmTaskSearch.Data[0].Name ?? string.Empty);

    }

    [Fact]
    public async Task SearchPpmAward()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var filter = new PpmAwardFilterInput() { Name = new StringFilterInput { Contains = "faculty" } };

        var result = await client.PpmAwardSearch.ExecuteAsync(filter, "K373D79");

        var data = result.ReadData();


        //With this change, this can return a list of awards, so we need to check the first one (They will all be the same, they can be linked to multiple projects when there is CostSharing (start with CS))
        data.PpmAwardByPpmAwardNumber.FirstOrDefault().ShouldNotBeNull();
        data.PpmAwardByPpmAwardNumber.FirstOrDefault()?.Name?.ShouldContain("Faculty");
        data.PpmAwardByPpmAwardNumber.FirstOrDefault()?.AwardStatus.ToString().ShouldBe("Active");




        data.PpmAwardSearch.ShouldNotBeNull();
        data.PpmAwardSearch.Data.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetPpmAward()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.PpmAward.ExecuteAsync("KL85D83");

        var data = result.ReadData();

        data.PpmAwardByPpmAwardNumber.ShouldNotBeNull();
        data.PpmAwardByPpmAwardNumber[0].PpmAwardNumber.ShouldBe("KL85D83");
        data.PpmAwardByPpmAwardNumber[0].GlFundCode.ShouldBe("20701");
        data.PpmAwardByPpmAwardNumber[0].GlPurposeCode.ShouldBe("62");
        data.PpmAwardByPpmAwardNumber[0].Name.ShouldBe("CAP Advanced testing and commercialization of novel defensin peptides and therapies for HLB control USDA FAIN 2021 70029 36056 KL85D83");
        data.PpmAwardByPpmAwardNumber[0].AwardStatus.ToString().ShouldBe("Active");
    }

    [Fact]
    public async Task GetTeamMembers()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.PpmProjectTeamMembers.ExecuteAsync("FPAFST4535", RoleNames.All);

        var data = result.ReadData();

        data.PpmProjectByNumber.ShouldNotBeNull();
        data.PpmProjectByNumber.TeamMembers.ShouldNotBeEmpty();
        data.PpmProjectByNumber.TeamMembers.Count.ShouldBe(2);
        data.PpmProjectByNumber.TeamMembers[0].RoleName.ShouldBe(RoleNames.PrincipalInvestigator);
        data.PpmProjectByNumber.TeamMembers[0].Person.ShouldNotBeNull();
        data.PpmProjectByNumber.TeamMembers[0].Person.FirstName.ShouldBe("Selina");
        data.PpmProjectByNumber.TeamMembers[1].RoleName.ShouldBe(RoleNames.ProjectManager);

    }

    [Fact]
    public async Task GetTeamMemberProjectManager()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.PpmProjectTeamMembers.ExecuteAsync("FPAFST4535", RoleNames.ProjectManager);

        var data = result.ReadData();

        data.PpmProjectByNumber.ShouldNotBeNull();
        data.PpmProjectByNumber.TeamMembers.ShouldNotBeEmpty();
        data.PpmProjectByNumber.TeamMembers.Count.ShouldBe(1);
        data.PpmProjectByNumber.TeamMembers[0].RoleName.ShouldBe(RoleNames.ProjectManager);
        data.PpmProjectByNumber.TeamMembers[0].Person.ShouldNotBeNull();
        data.PpmProjectByNumber.TeamMembers[0].Person.FirstName.ShouldBe("Lori");

    }
    [Fact]
    public async Task GetPpmAwardShouldReturnStartAndEndDates()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var result = await client.PpmAward.ExecuteAsync("K371F23");

        var data = result.ReadData();

        data.PpmAwardByPpmAwardNumber.ShouldNotBeNull();
        data.PpmAwardByPpmAwardNumber[0].StartDate.ShouldNotBeNull();
        data.PpmAwardByPpmAwardNumber[0].EndDate.ShouldNotBeNull();
        //data.PpmAwardByPpmAwardNumber[0].AwardStatus.ToString().ShouldBe("UnderAmendment"); // maybe this will change if the test is run in the future
        //data.PpmAwardByPpmAwardNumber[0].StartDate.ShouldBe("2022-07-01");
        //data.PpmAwardByPpmAwardNumber[0].EndDate.ShouldBe("2024-04-30");
    }

    [Fact]
    public async Task SearchPpmFundingSource()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        var filter = new PpmFundingSourceFilterInput() { Name = new StringFilterInput { Contains = "science" } };
        var result = await client.PpmFundingSourceSearch.ExecuteAsync(filter, "K103042");

        var data = result.ReadData();

        data.PpmFundingSourceByNumber.ShouldNotBeNull();
        data.PpmFundingSourceByNumber?.Name?.ShouldContain("SCIENCE");

        data.PpmFundingSourceSearch.ShouldNotBeNull();
        data.PpmFundingSourceSearch.Data.ShouldNotBeEmpty();
    }
}