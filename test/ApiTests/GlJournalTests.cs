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


        var result = await client.GlJournalRequestStatus.ExecuteAsync("77484d1a-91ed-4476-aaff-f28fb139ccc8");

        var data = result.ReadData();

        data.GlJournalRequestStatus.ShouldNotBeNull();
        data.GlJournalRequestStatus.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Complete);
        data.GlJournalRequestStatus.RequestStatus.ConsumerReferenceId.ShouldBe("CAES-Payments");
    }

    [Fact]
    public async Task ValidJournalStatusBySlothTransactionId()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);


        var result = await client.GlJournalRequestStatusByConsumerTracking.ExecuteAsync("AA314F00-C308-48XF-RF85-C8Ao7FD43199");

        var data = result.ReadData();

        data.GlJournalRequestStatusByConsumerTracking.ShouldNotBeNull();
        data.GlJournalRequestStatusByConsumerTracking.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Complete);
        data.GlJournalRequestStatusByConsumerTracking.RequestStatus.ConsumerReferenceId.ShouldBe("CAES-Payments");
        data.GlJournalRequestStatusByConsumerTracking.ProcessingResult.Jobs[0].JobReport.ShouldStartWith("UCD Primary Ledger");
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
                JournalSourceName = "UCD SLOTH",
                JournalCategoryName = "UCD Recharge",
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
                JournalSourceName = "UCD SLOTH",
                JournalCategoryName = "UCD Recharge",
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