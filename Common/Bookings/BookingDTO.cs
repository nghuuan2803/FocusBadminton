using Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Bookings
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string? MemberName { get; set; }
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string? PayMethodName { get; set; }
        public int? TeamId { get; set; }
        public string? TeamName { get; set; }
        public BookingType Type { get; set; } = BookingType.InDay;
        public DateTimeOffset? ApprovedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public double Amount { get; set; }
        public double EstimateCost { get; set; }
        public double Deposit { get; set; }
        public int? VoucherId { get; set; }
        public int? PromotionId { get; set; }
        public double Discount { get; set; }

        public DateTimeOffset? PausedDate { get; set; }
        public DateTimeOffset? ResumeDate { get; set; }

        [MaxLength(250)]
        public string? Note { get; set; }
        [MaxLength(250)]
        public string? AdminNote { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public virtual ICollection<BookingItem>? Details { get; set; }

        public string? PaymentLink { get; set; }
    }
}
