namespace Shared.Vouchers
{
    public class VoucherDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DiscountType { get; set; } = "Percent"; 
        public double Value { get; set; }
        public double MaximumValue { get; set; }
        public int? VoucherTemplateId { get; set; }
        public string? AccountId { get; set; }
        public DateTimeOffset? Expiry { get; set; }
        public bool IsUsed { get; set; }
        public string? Code { get; set; }
    }
}
