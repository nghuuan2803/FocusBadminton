using Shared.Members;
using System.ComponentModel.DataAnnotations;

namespace Shared.Teams
{
    public class TeamDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? TeamTierId { get; set; }
        public int? TierName { get; set; }
        public int LeaderId { get; set; }
        public string? LeaderName { get; set; }
        public double TeamPoints { get; set; }
        public double RewardPoints { get; set; }
        [MaxLength(500)]
        public string? Image { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual IEnumerable<MemberDTO>? Members { get; set; }
    }
}
