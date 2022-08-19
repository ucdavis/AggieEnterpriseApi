namespace AggieEnterpriseApi.Types;

public class PpmSegments
{
    public PpmSegments(string project, string task, string organization, string expenditureType,
        string? award = null, string? fundingSource? = null)
    {
        Project = project;
        Task = task;
        Organization = organization;
        ExpenditureType = expenditureType;
        Award = award;
        FundingSource = fundingSource;
    }

    /// <summary>
    /// Required: Managed Project Number
    /// </summary>
    public string Project { get; set; }

    /// <summary>
    /// Required: Task ID. Must belong to Project and be a chargeable task
    /// </summary>
    public string Task { get; set; }

    /// <summary>
    /// Required: Organization for which the expense is being incurred. Aligns with the GL Financial Department segment.
    /// </summary>
    public string Organization { get; set; }

    /// <summary>
    /// Required: Type of expense being charged to the project. Aligns with the GL Account segment.
    /// </summary>
    public string ExpenditureType { get; set; }

    /// <summary>
    /// Optional: Award for Sponsored projects only
    /// </summary>
    public string Award { get; set; }

    /// <summary>
    /// Optional: Award funding source for Sponsored projects only
    /// </summary>
    public string FundingSource { get; set; }

    public string ToSegmentString()
    {
        if (Award == null && FundingSource == null)
        {
            return $"{Project}-{Task}-{Organization}-{ExpenditureType}";
        }

        return $"{Project}-{Task}-{Organization}-{ExpenditureType}-{Award}-{FundingSource}";
    }
}