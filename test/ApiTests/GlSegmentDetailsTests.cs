using System;
using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
using ApiTests.Setup;
using Shouldly;
using Xunit;

namespace ApiTests;

public class GlSegmentDetailsTests : TestBase
{
    [Fact]
    public async Task GetDetails()
    {
        var client = AggieEnterpriseApi.GraphQlClient.Get(GraphQlUrl, Token);

        var result = await client.CoaDetails.ExecuteAsync(entity: "3110", fund: "12100", dept: "ADNO003", account: "775000", purpose:"45");
        
        var data = result.ReadData();
        
        data.ShouldNotBeNull();

        data.ErpEntity.ShouldNotBeNull();
        data.ErpEntity.Code.ShouldBe("3110");
        data.ErpEntity.Name.ShouldBe("UC Davis Campus Excluding School of Health");

        data.ErpFund.ShouldNotBeNull();        
        data.ErpFund.Code.ShouldBe("12100");
        data.ErpFund.Name.ShouldBe("SalesandServices Rate Based");

        data.ErpFinancialDepartment.ShouldNotBeNull();
        data.ErpFinancialDepartment.Code.ShouldBe("ADNO003");
        data.ErpFinancialDepartment.Name.ShouldBe("ADNO Deans Office Admin");

        data.ErpAccount.ShouldNotBeNull();
        data.ErpAccount.Code.ShouldBe("775000");
        data.ErpAccount.Name.ShouldBe("UCD Recharge Revenue");

        data.ErpPurpose.ShouldNotBeNull();
        data.ErpPurpose.Code.ShouldBe("45");
        data.ErpPurpose.Name.ShouldBe("AES Research");

    }


}