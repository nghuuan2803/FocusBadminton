using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Booking : BaseAuditableEntity<int>
    {
        public int MemberId { get; set; }
        public Member? Member { get; set; }

        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public BookingType Type { get; set; } = BookingType.InDay;
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public double Amount { get; set; }
        public double Deposit { get; set; }
        public int? VoucherId { get; set; } = null;
        public Voucher? Voucher { get; set; }
        public int? PromotionId { get; set; } = null;
        public Promotion? Promotion { get; set; }
        public double Discount { get; set; }

        public DateTimeOffset? PausedDate { get; set; }
        public DateTimeOffset? ResumeDate { get; set; }

        [MaxLength(250)]
        public string? Note { get; set; }
        [MaxLength(250)]
        public string? AdminNote { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public virtual ICollection<BookingDetail>? Details { get; set; }
    }
}
