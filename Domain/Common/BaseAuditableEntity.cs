using System.ComponentModel.DataAnnotations;

namespace Domain.Common
{
    public abstract class BaseAuditableEntity<T> : BaseEntity<T>
    {
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

        [MaxLength(36)]
        public string CreatedBy { get; set; } = "system";

        public DateTimeOffset? UpdatedAt { get; set; }

        [MaxLength(36)]
        public string? UpdatedBy { get; set; }
    }
}
