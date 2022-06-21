using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;


namespace ApiTests;

public class GlJournalTests : TestBase
{
    [Fact]
    public async Task ValidJournalStatus()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(TestData.GraphQlUrl, Token);

        var result = await client.GlJournalRequestStatus.ExecuteAsync("5a58177c-f47a-4b9c-88b9-743f3730085f");

        var data = result.ReadData();

        data.GlJournalRequestStatus.ShouldNotBeNull();
        data.GlJournalRequestStatus.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Complete);
        data.GlJournalRequestStatus.RequestStatus.ConsumerReferenceId.ShouldBe("CONSUMER_BATCH_NBR");
    }
}