namespace Shared.Teams
{
    public class RemoveMemberRequest
    {
        public int TeamId { get; set; }
        public int MemberId { get; set; }
        public int LeaderId { get; set; }
    }
}
