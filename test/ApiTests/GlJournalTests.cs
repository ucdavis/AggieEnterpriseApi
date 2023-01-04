using System;
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
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        // TODO: get better example request that wasn't errored out
        var result = await client.GlJournalRequestStatus.ExecuteAsync(new Guid( "22374301-0428-4356-847e-cb7ddaebf661"));

        var data = result.ReadData();

        data.GlJournalRequestStatus.ShouldNotBeNull();
        data.GlJournalRequestStatus.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Error);
        data.GlJournalRequestStatus.RequestStatus.ConsumerReferenceId.ShouldBe("ORDER_12345");
    }

    [Fact]
    public async Task CreateValidJournal()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var newJournalEntry = await client.GlJournalRequest.ExecuteAsync(new GlJournalRequestInput
        {
            Header = new ActionRequestHeaderInput
            {
                ConsumerTrackingId = Guid.NewGuid().ToString(),
                ConsumerReferenceId = "ORDER_12345",
                ConsumerNotes = "Invoices for CAES Payments",
                BoundaryApplicationName = "CAES Payments"
            },
            Payload = new GlJournalInput
            {
                JournalSourceName = "BOUNDARY_APP_1",
                JournalCategoryName = "INTERCOMPANY_REVENUE",
                JournalName = "Recharges July 2023",
                JournalReference = "ORDER_12345",
                JournalLines = new[]
                {
                    new GlJournalLineInput
                    {
                        GlSegmentString = "3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000",
                        DebitAmount = 100.00m,
                        ExternalSystemIdentifier = "ITEMX",
                        ExternalSystemReference = "CYBERSOURCE-Deposit",
                    },
                    new GlJournalLineInput
                    {
                        GlSegmentString = "3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000",
                        CreditAmount = 100.00m,
                        ExternalSystemIdentifier = "ITEMX",
                        ExternalSystemReference = "CYBERSOURCE-Deposit",
                    }
                }
            }
        });
        
        var data = newJournalEntry.ReadData();

        // newly created journal goes right into pending status
        data.GlJournalRequest.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Pending);
        data.GlJournalRequest.ValidationResults.ShouldBeNull(); // shouldn't be any errors
    }

    [Fact]
    public async Task CreateInValidJournal()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var newJournalEntry = await client.GlJournalRequest.ExecuteAsync(new GlJournalRequestInput
        {
            Header = new ActionRequestHeaderInput
            {
                ConsumerTrackingId = Guid.NewGuid().ToString(),
                ConsumerReferenceId = "ORDER_12345",
                ConsumerNotes = "Invoices for CAES Payments",
                BoundaryApplicationName = "CAES Payments"
            },
            Payload = new GlJournalInput
            {
                JournalSourceName = "BOUNDARY_APP_1",
                JournalCategoryName = "INTERCOMPANY_REVENUE",
                JournalName = "Recharges July 2023",
                JournalReference = "ORDER_12345",
                JournalLines = new[]
                {
                    new GlJournalLineInput
                    {
                        GlSegmentString = "3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000",
                        DebitAmount = 100.01m,
                        ExternalSystemIdentifier = "ITEMX",
                        ExternalSystemReference = "CYBERSOURCE-Deposit",
                    },
                    new GlJournalLineInput
                    {
                        GlSegmentString = "3110-13U20-ADNO003-410000-43-000-0000000000-000000-0000-000000-000000",
                        CreditAmount = 100.00m,
                        ExternalSystemIdentifier = "ITEMX",
                        ExternalSystemReference = "CYBERSOURCE-Deposit",
                    }
                }
            }
        });

        var data = newJournalEntry.ReadData();

        
        data.GlJournalRequest.ShouldNotBeNull();
        data.GlJournalRequest.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Rejected);
        data.GlJournalRequest.ValidationResults.ShouldNotBeNull();
        data.GlJournalRequest.ValidationResults.ErrorMessages.ShouldNotBeNull();
        data.GlJournalRequest.ValidationResults.ErrorMessages.Count.ShouldBe(1);
        data.GlJournalRequest.ValidationResults.ErrorMessages[0].ShouldStartWith("Credits and debits of journal lines must balance to zero.");
        data.GlJournalRequest.ValidationResults.MessageProperties.ShouldNotBeNull();
        data.GlJournalRequest.ValidationResults.MessageProperties.Count.ShouldBe(1);
        data.GlJournalRequest.ValidationResults.MessageProperties[0].ShouldBe("[creditAmount | debitAmount]");
        // data.GlJournalRequest.ValidationResults
    }
}