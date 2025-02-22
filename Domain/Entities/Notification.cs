using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Notification : BaseEntity<int>
    {
        [MaxLength(46)]
        public string? Receiver { get; set; }
        [MaxLength(100)]
        public required string Title { get; set; }
        [MaxLength(300)]
        public string? Content { get; set; }
        [MaxLength(50)]
        public string? Data { get; set; }
        public bool IsChecked { get; set; }
        public int Type { get; set; }
        public enum NotificationType
        {
            BookingConfirm,
            BookingRefuse,
            BookingCancel,
            ReceiveVoucher,
            VoucherExpire,
            NewPromotion,
        }
    }
}
