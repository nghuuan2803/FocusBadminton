using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Account : IdentityUser<string>
    {
        public double PersonalPoints { get; set; }
        public double RewardPoints { get; set; }
        [MaxLength(100)]
        public string? Avatar { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        [MaxLength(36)]
        public string CreatedBy { get; set; } = "System";

        public DateTimeOffset? UpdatedAt { get; set; }

        [MaxLength(36)]
        public string? UpdatedBy { get; set; }

        public DateTime? DeleteDate { get; set; }
    }
}
