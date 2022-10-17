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
    public async Task ConvertKfsToCoaPpm()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.KfsConvertAccount.ExecuteAsync("3", "FRMRATE", null);
        var data = result.ReadData();
        data.KfsConvertAccount.ShouldNotBeNull();
        data.KfsConvertAccount.MappingFound.ShouldBeTrue();
        data.KfsConvertAccount.PpmSegments.ShouldNotBeNull();
        data.KfsConvertAccount.GlSegments.ShouldBeNull();
    }

    [Fact]
    public async Task SearchLocation1()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var searchParms = new ErpInstitutionLocationFilterInput();
        searchParms.SearchCommon = new SearchCommonInputs();
        searchParms.SearchCommon.IncludeTotalResultCount = true;
        
        searchParms.LocationCode = new StringFilterInput { Contains = "Mrak%38" };

        var result = await client.ErpInstitutionLocationSearch.ExecuteAsync(searchParms, "Mrak%38");
        var data = result.ReadData();
        data.ErpInstitutionLocationSearch.ShouldNotBeNull();
        data.ErpInstitutionLocationSearch.Metadata.ShouldNotBeNull();
        data.ErpInstitutionLocationSearch.Metadata.TotalResultCount.ShouldBe(2);

        data.ErpInstitutionLocationSearch.Data[1].LocationCode.ShouldBe("Mrak Hall, Room 38");

        data.ErpInstitutionLocationByCode.ShouldBeNull();

    }

    [Fact]
    public async Task SearchLocation2()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var searchParms = new ErpInstitutionLocationFilterInput();
        searchParms.SearchCommon = new SearchCommonInputs();
        searchParms.SearchCommon.IncludeTotalResultCount = true;

        searchParms.LocationCode = new StringFilterInput { Contains = "Mrak%38" };

        var result = await client.ErpInstitutionLocationSearch.ExecuteAsync(searchParms, "Mrak Hall, Room 0038C");
        var data = result.ReadData();
        data.ErpInstitutionLocationSearch.ShouldNotBeNull();
        data.ErpInstitutionLocationSearch.Metadata.ShouldNotBeNull();
        data.ErpInstitutionLocationSearch.Metadata.TotalResultCount.ShouldBe(2);

        data.ErpInstitutionLocationSearch.Data[1].LocationCode.ShouldBe("Mrak Hall, Room 38");

        data.ErpInstitutionLocationByCode.ShouldNotBeNull();
        data.ErpInstitutionLocationByCode.LocationCode.ShouldBe("Mrak Hall, Room 0038C");
    }

    [Fact]
    public async Task SearchCategory1()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var searchParms = new ScmPurchasingCategoryFilterInput();
        searchParms.SearchCommon = new SearchCommonInputs();
        searchParms.SearchCommon.IncludeTotalResultCount = true;

        searchParms.Name = new StringFilterInput { Contains = "paper%prod" };

        var result = await client.ScmPurchasingCategorySearch.ExecuteAsync(searchParms);
        var data = result.ReadData();
        data.ScmPurchasingCategorySearch.ShouldNotBeNull();
        data.ScmPurchasingCategorySearch.Metadata.ShouldNotBeNull();
        data.ScmPurchasingCategorySearch.Metadata.TotalResultCount.ShouldBe(1);

        data.ScmPurchasingCategorySearch.Data[0].Name.ShouldBe("Paper products");
        data.ScmPurchasingCategorySearch.Data[0].Code.ShouldBe("14110000");

    }

    [Fact]
    public async Task SearchCategory2()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var searchParms = new ScmPurchasingCategoryFilterInput();
        searchParms.SearchCommon = new SearchCommonInputs();
        searchParms.SearchCommon.IncludeTotalResultCount = true;

        searchParms.Name = new StringFilterInput { Contains = "Test%123" };

        var result = await client.ScmPurchasingCategorySearch.ExecuteAsync(searchParms);
        var data = result.ReadData();
        data.ScmPurchasingCategorySearch.ShouldNotBeNull();
        data.ScmPurchasingCategorySearch.Metadata.ShouldNotBeNull();
        data.ScmPurchasingCategorySearch.Metadata.TotalResultCount.ShouldBe(1);

        data.ScmPurchasingCategorySearch.Data[0].Name.ShouldBe("Test 123");
        data.ScmPurchasingCategorySearch.Data[0].Code.ShouldBe("Test 123");
        data.ScmPurchasingCategorySearch.Data[0].StartDateActive.ShouldNotBeNull();
        data.ScmPurchasingCategorySearch.Data[0].EndDateActive.ShouldNotBeNull();
    }

    [Fact]
    public async Task CategoryCode()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.ScmPurchasingCategoryByCode.ExecuteAsync("14110000");
        var data = result.ReadData();
        data.ScmPurchasingCategoryByCode.ShouldNotBeNull();
        data.ScmPurchasingCategoryByCode.Name.ShouldBe("Paper products");
        data.ScmPurchasingCategoryByCode.Code.ShouldBe("14110000");

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
        address.Location?.CountryCode.ShouldBe("US");
    }

    [Fact]
    public async Task SearchVendor1()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);
        var filter = new ScmSupplierFilterInput { Name = new StringFilterInput{ Contains = "Fisher"} };
        var result = await client.SupplierNameAndNumberSupplierSearch.ExecuteAsync(filter, "426"); 

        var data = result.ReadData();
        data.ShouldNotBeNull();
        data.ScmSupplierByNumber.ShouldNotBeNull();
        data.ScmSupplierByNumber.Name.ShouldBe("FISHER SCIENTIFIC COMPANY LLC");
        data.ScmSupplierByNumber.SupplierNumber.ShouldBe(426);

        data.ScmSupplierSearch.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.ShouldNotBeNull();
        //data.ScmSupplierSearch.Data.Count().ShouldBe(15); //Can expect this to change when the data changes
        data.ScmSupplierSearch.Data.Where(a => a.SupplierNumber == 426).FirstOrDefault().ShouldNotBeNull();
    }

    [Fact]
    public async Task SearchVendor2()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);
        var filter = new ScmSupplierFilterInput { Name = new StringFilterInput { Contains = "FISHER SCIENTIFIC COMPANY LLC" } };
        var result = await client.SupplierNameAndNumberSupplierSearch.ExecuteAsync(filter, "426");

        var data = result.ReadData();
        data.ShouldNotBeNull();
        data.ScmSupplierByNumber.ShouldNotBeNull();
        data.ScmSupplierByNumber.Name.ShouldBe("FISHER SCIENTIFIC COMPANY LLC");
        data.ScmSupplierByNumber.SupplierNumber.ShouldBe(426);

        data.ScmSupplierSearch.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.Count().ShouldBe(1);
        data.ScmSupplierSearch.Data.Where(a => a.SupplierNumber == 426).FirstOrDefault().ShouldNotBeNull();

    }

    [Fact]
    public async Task SearchVendor3()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);
        var filter = new ScmSupplierFilterInput { Name = new StringFilterInput { Contains = "FISHER SCIENTIFIC COMPANY LLC" } };
        var result = await client.SupplierNameAndNumberSupplierSearch.ExecuteAsync(filter, "FISHER SCIENTIFIC COMPANY LLC");

        var data = result.ReadData();
        data.ShouldNotBeNull();
        data.ScmSupplierByNumber.ShouldBeNull();
        //data.ScmSupplierByNumber.Name.ShouldBe("FISHER SCIENTIFIC COMPANY LLC");
        //data.ScmSupplierByNumber.SupplierNumber.ShouldBe(426);

        data.ScmSupplierSearch.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.Count().ShouldBe(1);
        data.ScmSupplierSearch.Data.Where(a => a.SupplierNumber == 426).FirstOrDefault().ShouldNotBeNull();
    }

    [Fact]
    public async Task SearchVendor4()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);
        var filter = new ScmSupplierFilterInput { Name = new StringFilterInput { Contains = "426" } };
        var result = await client.SupplierNameAndNumberSupplierSearch.ExecuteAsync(filter, "426");

        var data = result.ReadData();
        data.ShouldNotBeNull();
        data.ScmSupplierByNumber.ShouldNotBeNull();
        data.ScmSupplierByNumber.Name.ShouldBe("FISHER SCIENTIFIC COMPANY LLC");
        data.ScmSupplierByNumber.SupplierNumber.ShouldBe(426);

        data.ScmSupplierSearch.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.Count().ShouldBe(0);
        //data.ScmSupplierSearch.Data.Where(a => a.SupplierNumber == 426).FirstOrDefault().ShouldNotBeNull();
    }

    [Fact]
    public async Task SearchVendor5()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);
        var filter = new ScmSupplierFilterInput { Name = new StringFilterInput { Contains = "No Match" } };
        var result = await client.SupplierNameAndNumberSupplierSearch.ExecuteAsync(filter, "No Match");

        var data = result.ReadData();
        data.ShouldNotBeNull();
        data.ScmSupplierByNumber.ShouldBeNull();
        //data.ScmSupplierByNumber.Name.ShouldBe("FISHER SCIENTIFIC COMPANY LLC");
        //data.ScmSupplierByNumber.SupplierNumber.ShouldBe(426);

        data.ScmSupplierSearch.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.ShouldNotBeNull();
        data.ScmSupplierSearch.Data.Count().ShouldBe(0);
        //data.ScmSupplierSearch.Data.Where(a => a.SupplierNumber == 426).FirstOrDefault().ShouldNotBeNull();
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

    [Fact]
    public async Task LookupStatus()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var saveId = Guid.NewGuid().ToString();

        var result = await client.ScmPurchaseRequisitionRequestStatus.ExecuteAsync("e54836a7-5171-4f9c-96c1-5d70c5612ae9");

        var data = result.ReadData();

        data.ScmPurchaseRequisitionRequestStatus.ShouldNotBeNull();
        data.ScmPurchaseRequisitionRequestStatus.RequestStatus.RequestStatus.ShouldBe(RequestStatus.Pending);
        data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ConsumerReferenceId.ShouldBe("ACRU-EHIT218");
        data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ConsumerTrackingId.ShouldBe("219e65a1-94aa-45f3-a60b-76cd930d567c");
        data.ScmPurchaseRequisitionRequestStatus.ValidationResults.ShouldBeNull();
        data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ErrorMessages.ShouldNotBeNull();
        data.ScmPurchaseRequisitionRequestStatus.RequestStatus.ErrorMessages.Count.ShouldBe(0);

    }

    [Fact]
    public async Task SearchUnitOfMeasure()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var searchParms = new ErpUnitOfMeasureFilterInput();
        searchParms.SearchCommon = new SearchCommonInputs();
        searchParms.SearchCommon.IncludeTotalResultCount = true;
        searchParms.SearchCommon.Limit = 200;

        searchParms.Name = new StringFilterInput { Contains = "%" }; //Grab them all

        var result = await client.ErpUnitOfMeasureSearch.ExecuteAsync(searchParms);
        var data = result.ReadData();
        data.ErpUnitOfMeasureSearch.ShouldNotBeNull();
        data.ErpUnitOfMeasureSearch.Metadata.ShouldNotBeNull();
        data.ErpUnitOfMeasureSearch.Metadata.TotalResultCount.ShouldBe(148);
        data.ErpUnitOfMeasureSearch.Metadata.NextStartIndex.ShouldBeNull();

        data.ErpUnitOfMeasureSearch.Data[0].Name.ShouldBe("Each");
        data.ErpUnitOfMeasureSearch.Data[0].UomCode.ShouldBe("EA");

    }
}