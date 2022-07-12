using System.Threading.Tasks;
using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Validation;
using ApiTests.Setup;
using Xunit;
using Shouldly;
using Microsoft.Extensions.Configuration;


namespace ApiTests;
public class ValidationHelperTests : TestBase
{
    [Fact]
    public void ValidGlString()
    {
        var glString = "3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000";
        
        FinancialChartValidation.IsValidGlSegmentString(glString).ShouldBeTrue();
        FinancialChartValidation.GetFinancialChartStringType(glString).ShouldBe(FinancialChartStringType.Gl);
    }
    
    [Fact]
    public void InvalidGlString()
    {
        var glString = "3110-72160-9300202-775000-85-000ABC";
        
        FinancialChartValidation.IsValidGlSegmentString(glString).ShouldBeFalse();

        FinancialChartValidation.GetFinancialChartStringType(glString).ShouldBe(FinancialChartStringType.Invalid);
    }
    
    [Fact]
    public void ValidGlSegments()
    {
        var glString = "3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000";

        var segments = FinancialChartValidation.GetGlSegments(glString);
        
        segments.Entity.ShouldBe("3110");
        // TODO: validate rest of segments
    }

    [Fact]
    public void GlSegmentsToString()
    {
        var glString = "3110-72160-9300202-775000-85-000-0000000000-000000-0000-000000-000000";
        
        var segments = FinancialChartValidation.GetGlSegments(glString);

        var glString2 = segments.ToSegmentString();
        
        glString2.ShouldBe(glString);
    }
    
    [Fact]
    public void ValidPpmStringRequiredOnly()
    {
        var ppmString = "SP00000001-000001-0000000-000000";
        
        FinancialChartValidation.IsValidPpmSegmentString(ppmString).ShouldBeTrue();
        FinancialChartValidation.GetFinancialChartStringType(ppmString).ShouldBe(FinancialChartStringType.Ppm);
    }
    
    [Fact]
    public void ValidPpmStringFull()
    {
        var ppmString = "CP00000001-000001-0000000-000000-0000000-00000";
        
        FinancialChartValidation.IsValidPpmSegmentString(ppmString).ShouldBeTrue();
        FinancialChartValidation.GetFinancialChartStringType(ppmString).ShouldBe(FinancialChartStringType.Ppm);
    }
    
    [Fact]
    public void InvalidPpmStringRequiredOnly()
    {
        var ppmString = "SP0000000-FANCY-PROJECT";
        
        FinancialChartValidation.IsValidPpmSegmentString(ppmString).ShouldBeFalse();
        FinancialChartValidation.GetFinancialChartStringType(ppmString).ShouldBe(FinancialChartStringType.Invalid);
    }
    
    [Fact]
    public void ValidPpmSegments()
    {
        var ppmString = "CP00000001-000001-0000000-000000-0000000-00000";

        var segments = FinancialChartValidation.GetPpmSegments(ppmString);
        
        segments.Project.ShouldBe("CP00000001");
    }
    
    [Fact]
    public void PpmSegmentsToString()
    {
        var ppmString = "CP00000001-000001-0000000-000000-0000000-00000";
        
        var segments = FinancialChartValidation.GetPpmSegments(ppmString);

        var ppmString2 = segments.ToSegmentString();
        
        ppmString2.ShouldBe(ppmString);
    }
}