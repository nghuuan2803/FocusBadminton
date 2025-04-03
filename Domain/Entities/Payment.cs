using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Payment : BaseEntity<Guid>
    {
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public PaymentMethod Method { get; set; }
        public double Amount { get; set; }
        public PaymentType Type { get; set; } = PaymentType.Deposit;
        [MaxLength(100)]
        public string? Info { get; set; }

        [MaxLength(150)]
        public string? Note { get; set; }

        [MaxLength(100)]
        public string? Image { get; set; }
        public DateTime PaidAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? TransactionId { get; set; }
    }
}
