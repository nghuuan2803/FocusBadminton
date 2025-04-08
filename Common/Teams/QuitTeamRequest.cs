namespace Shared.Teams
{
    public class QuitTeamRequest
    {
        public required int TeamId { get; set; }
        public required int MemberId { get; set; }
        public string? Reason { get; set; }
    }
}
