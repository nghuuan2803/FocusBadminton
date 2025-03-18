namespace Shared.Members
{
    public class MemberDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public double Contributed { get; set; }
        public string? Email { get; set; }
        public string? AccountId { get; set; }
        public int? CurrentTeamId { get; set; }
        public DateTimeOffset? JoinedTeamAt { get; set; }
        public string? Gender { get; set; }
        public DateTime? DoB { get; set; }
        public string? Address { get; set; }
    }
}
