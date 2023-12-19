using System.Text.RegularExpressions;
using AggieEnterpriseApi.Types;

namespace AggieEnterpriseApi.Validation;

public enum FinancialChartStringType
{
    Gl,
    Ppm,
    Invalid
};

// helper methods to convert COA string to segments, validate structure, and determine type
public static class FinancialChartValidation
{
    public static readonly Regex GlSegmentStringPattern =
        new Regex(
            "^[0-9]{3}[0-9AB]-[0-9A-Z]{5}-[0-9A-Z]{7}-[0-9A-Z]{6}-[0-9][0-9A-Z]-[0-9A-Z]{3}-[0-9A-Z]{10}-[0-9A-Z]{6}-0000-000000-000000$");

    public static readonly Regex PpmSegmentStringPattern =
        new Regex("^[0-9A-Z]{10}-[0-9A-Z]{6}-[0-9A-Z]{7}-[0-9A-Z]{6}(-[0-9A-Z]{7}-[0-9A-Z]{5,10})?$");


    public static bool IsValidGlSegmentString(string glSegmentString)
    {
        return GlSegmentStringPattern.IsMatch(glSegmentString);
    }

    public static bool IsValidPpmSegmentString(string ppmSegmentString)
    {
        return PpmSegmentStringPattern.IsMatch(ppmSegmentString);
    }

    public static FinancialChartStringType GetFinancialChartStringType(string financialChartString)
    {
        if (IsValidGlSegmentString(financialChartString))
        {
            return FinancialChartStringType.Gl;
        }
        else if (IsValidPpmSegmentString(financialChartString))
        {
            return FinancialChartStringType.Ppm;
        }
        else
        {
            return FinancialChartStringType.Invalid;
        }
    }

    public static GlSegments GetGlSegments(string glSegmentString)
    {
        if (!IsValidGlSegmentString(glSegmentString))
        {
            throw new ArgumentException("Invalid GL segment string");
        }

        var segments = glSegmentString.Split('-');

        return new GlSegments(segments[0], segments[1], segments[2], segments[3], segments[4], segments[5], segments[6],
            segments[7]);
    }

    public static PpmSegments GetPpmSegments(string ppmSegmentString)
    {
        if (!IsValidPpmSegmentString(ppmSegmentString))
        {
            throw new ArgumentException("Invalid PPM segment string");
        }

        var segments = ppmSegmentString.Split('-');

        // There are two possible PPM segment strings - either 4 or 6 segment varieties.
        var isRequiredOnly = segments.Length == 4;

        if (isRequiredOnly)
        {
            return new PpmSegments(segments[0], segments[1], segments[2], segments[3]);
        }
        else
        {
            // full string, populate everything
            return new PpmSegments(segments[0], segments[1], segments[2], segments[3], segments[4], segments[5]);
        }
    }
}