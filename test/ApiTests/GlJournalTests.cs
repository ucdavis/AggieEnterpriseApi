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
    public async Task InValidJournalStatus()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

        // TODO: get better example request that wasn't errored out
        var result = await client.GlJournalRequestStatus.ExecuteAsync(new Guid("130fba74-24d0-4838-8d35-831812020893"));

        var data = result.ReadData();

        data.GlJournalRequestStatus.ShouldNotBeNull();
        data.GlJournalRequestStatus.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Error);
        data.GlJournalRequestStatus.RequestStatus.ConsumerReferenceId.ShouldBe("Aggie Enterprise Test Recharge");
        data.GlJournalRequestStatus.ProcessingResult.ShouldNotBeNull();
        data.GlJournalRequestStatus.ProcessingResult.Jobs.ShouldNotBeNull();
        data.GlJournalRequestStatus.ProcessingResult.Jobs.Count.ShouldBe(3);
        data.GlJournalRequestStatus.ProcessingResult.Jobs[0].JobStatus.ShouldBe("PROCESSED");
        data.GlJournalRequestStatus.ProcessingResult.Jobs[1].JobStatus.ShouldBe("ERROR");
        data.GlJournalRequestStatus.ProcessingResult.Jobs[1].JobReport.ShouldStartWith("{\"G_REQUEST_ID\":1323230,\"G_BUSINESS_UNIT\":\"UCD CGA Business Unit\",\"G_PROCESS_MODE\":\"Importing and processing transactions\",\"G_TRANSACTION_STATUS\":\"Not previously imported\"");
    }

    [Fact]
    public async Task CreateValidJournal()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

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
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, TokenEndpoint, ConsumerKey, ConsumerSecret, $"{ScopeApp}-{ScopeEnv}");

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