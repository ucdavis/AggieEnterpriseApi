namespace AggieEnterpriseApi.Types;

public class GlSegments
{
    public GlSegments(string entity, string fund, string department, string account, string purpose, string program, string project, string activity)
    {
        Account = account;
        Activity = activity;
        Department = department;
        Entity = entity;
        Fund = fund;
        Program = program;
        Project = project;
        Purpose = purpose;
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
}