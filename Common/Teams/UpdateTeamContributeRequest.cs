namespace Shared.Teams
{
    public class UpdateTeamContributeRequest
    {
        public int TeamId { get; set; }
        public int LeaderId { get; set; }
        public Dictionary<int,double> MemberContributes { get; set; }
    }
}
