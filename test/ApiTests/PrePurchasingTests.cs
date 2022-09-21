using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;
using AggieEnterpriseApi.Types;

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
    


}