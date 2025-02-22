using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Role : IdentityRole<string>
    {
        public DateTimeOffset CreatedAt { get; set; }

        [MaxLength(36)]
        public string CreatedBy { get; set; } = "System";
        public DateTimeOffset? UpdatedAt { get; set; }
        [MaxLength(36)]
        public string? UpdatedBy { get; set; }
    }
}
