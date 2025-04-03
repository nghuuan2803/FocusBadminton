using Shared.Vouchers;

namespace Application.Common.Mappings
{
    public static class VoucherMapping
    {
        public static VoucherDTO ToVoucherDTO(Voucher voucher)
        {
            return new VoucherDTO
            {
                Id = voucher.Id,
                Name = voucher.Name,
                Description = voucher.Description,
                DiscountType = voucher.DiscountType,
                Value = voucher.Value,
                MaximumValue = voucher.MaximumValue,
                VoucherTemplateId = voucher.VoucherTemplateId,
                AccountId = voucher.AccountId,
                Expiry = voucher.Expiry,
                IsUsed = voucher.IsUsed,
                Code = voucher.Code
            };
        }

        public static VoucherTemplateDTO ToVoucherTemplateDTO(VoucherTemplate template)
        {
            return new VoucherTemplateDTO
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                DiscountType = template.DiscountType,
                Value = template.Value,
                MaximumValue = template.MaximumValue,
                Duration = template.Duration
            };
        }
    }
}
