namespace Shared.Teams
{
    public class AddMembersRequest
    {
        public int TeamId { get; set; }
        public int LeaderId { get; set; }
        public IEnumerable<TeamMember> Members { get; set; }

    }

    public class TeamMember
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }
}
