namespace Domain.Entities
{
    public class LeftHistory : BaseEntity<int>
    {
        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }
        public DateTimeOffset JoinedAt { get; set; }
        public DateTimeOffset? LeftAt { get; set; }
    }
}
