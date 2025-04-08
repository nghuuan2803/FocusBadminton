namespace Shared.Teams
{
    public class TeamMemberDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTimeOffset JoinTeamAt { get; set; }
        public bool IsLeader { get; set; }
        public double CurrentContribute { get; set; } = 0;
    }
}
