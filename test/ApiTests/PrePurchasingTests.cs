using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using AggieEnterpriseApi.Types;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ApiTests;
public class PrePurchasingTests : TestBase
{
    [Fact]
    public async Task ConvertKfsToCoa()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.KfsConvertAccount.ExecuteAsync("3", "CRU9033", null);
        var data = result.ReadData();
        data.KfsConvertAccount.ShouldNotBeNull();
        data.KfsConvertAccount.MappingFound.ShouldBeTrue();
        data.KfsConvertAccount.PpmSegments.ShouldBeNull();
        data.KfsConvertAccount.GlSegments.ShouldNotBeNull();

        var glSegments = new GlSegments(data.KfsConvertAccount.GlSegments.Entity, data.KfsConvertAccount.GlSegments.Fund, data.KfsConvertAccount.GlSegments.Department, "000000", data.KfsConvertAccount.GlSegments.Purpose ?? "00", data.KfsConvertAccount.GlSegments.Program ?? "000", data.KfsConvertAccount.GlSegments.Project ?? "0000000000", data.KfsConvertAccount.GlSegments.Activity ?? "000000");
        glSegments.ToSegmentString().ShouldBe("3110-13U02-ADNO006-000000-43-000-0000000000-000000-0000-000000-000000");

        var segments2 = new GlSegments(data.KfsConvertAccount.GlSegments).ToSegmentString();
        segments2.ShouldBe("3110-13U02-ADNO006-000000-43-000-0000000000-000000-0000-000000-000000");
    }

    [Fact]
    public async Task LookupKfsVendor()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput{ Eq = "83526" } };

        var result = await client.ScmSupplierSearch.ExecuteAsync(search);
        var data = result.ReadData();

        data.ScmSupplierSearch.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.First().Name.ShouldBe("DELL ENTERPRISES");
        data.ScmSupplierSearch.Data.First().SupplierNumber.ShouldBe(83526);
        data.ScmSupplierSearch.Data.First().SupplierNumber.ToString().ShouldBe("83526");
        var address = data.ScmSupplierSearch.Data.First().Sites.Where(a => a.Location?.City == "ROUND ROCK" && a.Location?.AddressLine2 == "ONE DELL WAY" && a.Location?.State == "TX").FirstOrDefault();
        address.ShouldNotBeNull();
        address.SupplierSiteCode.ShouldBe("PUR-4");
    }

    
    
    
    [Fact]
    public async Task CreateRequsition()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var saveId = Guid.NewGuid().ToString();

        var inputOrder = new ScmPurchaseRequisitionRequestInput
        {
            Header = new ActionRequestHeaderInput
            {
                ConsumerTrackingId = saveId,
                ConsumerReferenceId = "ACRU-EHIT218 - TEST",
                ConsumerNotes = "Workgroup: CRU Internal Workgroup",
                BoundaryApplicationName = "PrePurchasing",
                ConsumerId = "UCD Giving Service", //Remove after fixed 9/23/2022
            }
        };

        var search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput { Eq = "83526" } };
        var searchResult = await client.ScmSupplierSearch.ExecuteAsync(search);
        var searchData = searchResult.ReadData();

        inputOrder.Payload = new ScmPurchaseRequisitionInput
        {
            RequisitionSourceName = "UCD SLOTH",
            SupplierNumber = searchData.ScmSupplierSearch.Data.First().SupplierNumber.ToString(),
            SupplierSiteCode = searchData.ScmSupplierSearch.Data.First().Sites.Where(a => a.Location?.City == "ROUND ROCK" && a.Location?.AddressLine2 == "ONE DELL WAY" && a.Location?.State == "TX").First().SupplierSiteCode,
            RequesterEmailAddress = "jsylvestre@ucdavis.edu",
            Description = "ACRU-EHIT218 - TEST",
            Justification = "Print Toner for PrintsCharming 38 Mrak Dell eQuote #3000118141264",
        };

        var distributionResult = await client.KfsConvertAccount.ExecuteAsync("3", "CRU9033", null);
        var distributionData = distributionResult.ReadData();
        var segmentString = "";
        if(distributionData.KfsConvertAccount.GlSegments != null)
        {
            segmentString = new GlSegments(distributionData.KfsConvertAccount.GlSegments).ToSegmentString();
        }

        var Line1 = new ScmPurchaseRequisitionLineInput
        {
            Amount = 102.99m,
            Quantity = 1,
            UnitOfMeasure = "Each",
            ItemDescription = "Dell 2155cnd Black Toner",
            PurchasingCategoryName = "15000FAC", //Completely faked
            UnitPrice = 102.99m,
            Distributions = new[]
            {
                new ScmPurchaseRequisitionDistributionInput
                {
                    Amount = 102.99m,
                    GlSegmentString = segmentString, 
                }
            },
        };

        var Line2 = new ScmPurchaseRequisitionLineInput
        {
            Amount = 83.99m,
            Quantity = 1,
            UnitOfMeasure = "Each",
            ItemDescription = "Dell 2155cdn Cyan Toner",
            PurchasingCategoryName = "15000FAC", //Completely faked
            UnitPrice = 83.99m,
            Distributions = new[]
            {
                new ScmPurchaseRequisitionDistributionInput
                {
                    Amount = 83.99m,
                    GlSegmentString = segmentString,
                }
            },
        };

        var Line3 = new ScmPurchaseRequisitionLineInput
        {
            Amount = 101.99m,
            Quantity = 1,
            UnitOfMeasure = "Each",
            ItemDescription = "Dell 2155cdn Magenta Toner",
            PurchasingCategoryName = "15000FAC", //Completely faked
            UnitPrice = 101.99m,
            Distributions = new[]
            {
                new ScmPurchaseRequisitionDistributionInput
                {
                    Amount = 101.99m,
                    GlSegmentString = segmentString,
                }
            },
        };

        var Lines = new List<ScmPurchaseRequisitionLineInput>(3);
        Lines.Add(Line1);
        Lines.Add(Line2);
        Lines.Add(Line3);

        inputOrder.Payload.Lines = Lines;



        var NewOrderRequsition = await client.ScmPurchaseRequisitionCreate.ExecuteAsync(inputOrder);

        var data = NewOrderRequsition.ReadData();

        data.ScmPurchaseRequisitionCreate.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.RequestStatus.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.RequestStatus.RequestId.ShouldNotBeNull();
        //data.ScmPurchaseRequisitionCreate.RequestStatus.RequestStatus.ToString().ShouldBe("PENDING");
        data.ScmPurchaseRequisitionCreate.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Pending);
        data.ScmPurchaseRequisitionCreate.ValidationResults.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.ValidationResults.Valid.ShouldBeTrue();

    }


    [Fact]
    public async Task CreateInvalidRequsition()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var saveId = Guid.NewGuid().ToString();

        var inputOrder = new ScmPurchaseRequisitionRequestInput
        {
            Header = new ActionRequestHeaderInput
            {
                ConsumerTrackingId = saveId,
                ConsumerReferenceId = "ACRU-EHIT218 - TEST",
                ConsumerNotes = "Workgroup: CRU Internal Workgroup",
                BoundaryApplicationName = "PrePurchasing",
                ConsumerId = "UCD Giving Service", //Remove after fixed 9/23/2022
            }
        };

        var search = new ScmSupplierFilterInput { SupplierNumber = new StringFilterInput { Eq = "83526" } };
        var searchResult = await client.ScmSupplierSearch.ExecuteAsync(search);
        var searchData = searchResult.ReadData();

        inputOrder.Payload = new ScmPurchaseRequisitionInput
        {
            RequisitionSourceName = "UCD SLOTH",
            SupplierNumber = searchData.ScmSupplierSearch.Data.First().SupplierNumber.ToString(),
            SupplierSiteCode = searchData.ScmSupplierSearch.Data.First().Sites.Where(a => a.Location?.City == "ROUND ROCK" && a.Location?.AddressLine2 == "ONE DELL WAY" && a.Location?.State == "TX").First().SupplierSiteCode,
            RequesterEmailAddress = "jsylvestre@ucdavis.edu",
            Description = "ACRU-EHIT218 - TEST",
            Justification = "Print Toner for PrintsCharming 38 Mrak Dell eQuote #3000118141264",
        };

        var distributionResult = await client.KfsConvertAccount.ExecuteAsync("3", "CRU9033", null);
        var distributionData = distributionResult.ReadData();
        var segmentString = "";
        if (distributionData.KfsConvertAccount.GlSegments != null)
        {
            segmentString = new GlSegments(distributionData.KfsConvertAccount.GlSegments).ToSegmentString();
        }

        var Line1 = new ScmPurchaseRequisitionLineInput
        {
            Amount = 102.99m,
            Quantity = 1,
            UnitOfMeasure = "Each",
            ItemDescription = "Dell 2155cnd Black Toner",
            PurchasingCategoryName = "15000FAC", //Completely faked
            UnitPrice = 102.99m,
            Distributions = new[]
            {
                new ScmPurchaseRequisitionDistributionInput
                {
                    Amount = 302.99m, //Too big
                    GlSegmentString = segmentString,
                }
            },
        };

        var Line2 = new ScmPurchaseRequisitionLineInput
        {
            Amount = 83.99m,
            Quantity = 1,
            UnitOfMeasure = "Each",
            ItemDescription = "Dell 2155cdn Cyan Toner",
            PurchasingCategoryName = "15000FAC", //Completely faked
            UnitPrice = 83.99m,
            Distributions = new[]
            {
                new ScmPurchaseRequisitionDistributionInput
                {
                    Amount = 83.99m,
                    GlSegmentString = segmentString,
                }
            },
        };

        var Line3 = new ScmPurchaseRequisitionLineInput
        {
            Amount = 101.99m,
            Quantity = 1,
            UnitOfMeasure = "Each",
            ItemDescription = "Dell 2155cdn Magenta Toner",
            PurchasingCategoryName = "15000FAC", //Completely faked
            UnitPrice = 101.99m,
            Distributions = new[]
            {
                new ScmPurchaseRequisitionDistributionInput
                {
                    Amount = 101.99m,
                    GlSegmentString = segmentString,
                }
            },
        };

        var Lines = new List<ScmPurchaseRequisitionLineInput>(3);
        Lines.Add(Line1);
        Lines.Add(Line2);
        Lines.Add(Line3);

        inputOrder.Payload.Lines = Lines;



        var NewOrderRequsition = await client.ScmPurchaseRequisitionCreate.ExecuteAsync(inputOrder);

        var data = NewOrderRequsition.ReadData();

        data.ScmPurchaseRequisitionCreate.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.RequestStatus.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.RequestStatus.RequestId.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Rejected);
        data.ScmPurchaseRequisitionCreate.ValidationResults.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.ValidationResults.Valid.ShouldBeFalse();
        data.ScmPurchaseRequisitionCreate.ValidationResults.ErrorMessages.ShouldNotBeNull();
        data.ScmPurchaseRequisitionCreate.ValidationResults.ErrorMessages.Count.ShouldBe(1);
        data.ScmPurchaseRequisitionCreate.ValidationResults.ErrorMessages.First().ShouldBe("Line amount (102.99) is not sum of distributions (302.99).");

    }
}