using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Team : BaseAuditableEntity<int>
    {
        [MaxLength(50)]
        public required string Name { get; set; }
        public int? TeamTierId { get; set; }
        public int LeaderId { get; set; }
        public Member? Leader { get; set; }
        public double TeamPoints { get; set; }
        public double RewardPoints { get; set; }
        [MaxLength(500)]
        public string? Image { get; set; }

        public virtual ICollection<Member>? Members { get; set; }
    }
}
