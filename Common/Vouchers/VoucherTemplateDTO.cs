using Shared.Enums;

namespace Shared.Vouchers
{
    public class VoucherTemplateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DiscountType { get; set; }
        public double Value { get; set; }
        public double MaximumValue { get; set; }
        public int Duration { get; set; }
    }
}
