namespace AggieEnterpriseApi.Types;

public class GlSegments
{
    public GlSegments(string entity, string fund, string department, string account, string purpose, string program = "000",
        string project = "0000000000", string activity = "000000", string interEntity = "0000", string flex1 = "000000", string flex2 = "000000")
    {
        Account = account;
        Activity = activity;
        Department = department;
        Entity = entity;
        Fund = fund;
        Program = program;
        Project = project;
        Purpose = purpose;

        InterEntity = interEntity;
        Flex1 = flex1;
        Flex2 = flex2;
    }

    /// <summary>
    /// Required: Nature of the transaction, expense, income, liability, etc...
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// Optional: 
    /// </summary>
    public string Activity { get; set; }

    /// <summary>
    /// Required: Financial department to which to charge a transaction.
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// Required: Entity to which to charge a transaction.
    /// </summary>
    public string Entity { get; set; }

    /// <summary>
    /// Required: Funding source to which to charge a transaction.
    /// </summary>
    public string Fund { get; set; }

    /// <summary>
    /// Optional: 
    /// </summary>
    public string Program { get; set; }

    /// <summary>
    /// Optional: 
    /// </summary>
    public string Project { get; set; }

    /// <summary>
    /// Required for Expenses: Functional purpose of the expense.
    /// </summary>
    public string Purpose { get; set; }

    /// <summary>
    /// Unused: Always 0000.
    /// </summary>
    public string InterEntity { get; set; }

    /// <summary>
    /// Unused: For future UCOP Reporting Requirements. Always 000000.
    /// </summary>
    public string Flex1 { get; set; }

    /// <summary>
    /// Unused: For future UCOP Reporting Requirements. Always 000000.
    /// </summary>
    public string Flex2 { get; set; }

    public string ToSegmentString()
    {
        return $"{Entity}-{Fund}-{Department}-{Account}-{Purpose}-{Program}-{Project}-{Activity}-{InterEntity}-{Flex1}-{Flex2}";
    }
}