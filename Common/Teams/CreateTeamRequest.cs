namespace Shared.Teams
{
    public class CreateTeamRequest
    {
        public required string Name { get; set; }
        public int LeaderId { get; set; }
        public string? Image { get; set; }
    }
}
