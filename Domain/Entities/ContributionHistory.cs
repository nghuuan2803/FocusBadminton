namespace Domain.Entities
{
    public class ContributionHistory : BaseEntity<int>
    {
        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }
        public double Amount { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}
