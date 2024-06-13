public class RoleNames
{
    public const string All = null;
    public const string PrincipalInvestigator = "Principal Investigator";
    public const string ProjectManager = "Project Manager";
    public const string ProjectAdministrator = "Project Administrator";
    public const string CoPrincipalInvestigator = "Co-Principal Investigator";
    public const string GrantsAdministrator = "Grants Administrator";
    public const string ProjectParticipant = "Project Participant";
    public const string TeamMember = "Team Member";


    public static List<string> Types = new List<string>
    {
        All,
        PrincipalInvestigator,
        CoPrincipalInvestigator,
        ProjectManager,
        ProjectAdministrator,
        ProjectParticipant,
        GrantsAdministrator,
        TeamMember
    };
}
