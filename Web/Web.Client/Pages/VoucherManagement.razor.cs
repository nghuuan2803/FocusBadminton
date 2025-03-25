using Shared.Vouchers;

namespace Web.Client.Pages
{
    public partial class VoucherManagement
    {
        private async Task HandleCreateTemplate()
        {
            if (string.IsNullOrWhiteSpace(newTemplate.Name))
            {
                await MessageService.Error("Tên mẫu không được để trống!");
                return;
            }
            if (newTemplate.Value <= 0)
            {
                await MessageService.Error("Giá trị phải lớn hơn 0!");
                return;
            }
            if (newTemplate.Duration <= 0)
            {
                await MessageService.Error("Thời hạn phải lớn hơn 0!");
                return;
            }

            isCreatingTemplate = true;
            try
            {
                var createdTemplate = await VouchersService.CreateVoucherTemplateAsync(newTemplate);
                if (createdTemplate != null)
                {
                    voucherTemplates.Add(createdTemplate);
                    newTemplate = new VoucherTemplateDTO
                    {
                        DiscountType = DiscountType.Percent,
                        Value = 0,
                        MaximumValue = 0,
                        Duration = 30
                    };
                    await MessageService.Success("Tạo mẫu voucher thành công!");
                }
                else
                {
                    await MessageService.Error("Không thể tạo mẫu voucher!");
                }
            }
            catch (HttpRequestException ex)
            {
                await MessageService.Error($"Lỗi khi tạo mẫu: {ex.Message} (Status: {ex.StatusCode})");
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi không xác định: {ex.Message}");
            }
            finally
            {
                isCreatingTemplate = false;
                StateHasChanged();
            }
        }
        private async Task HandleCreateVoucher()
        {
            if (voucherRequest.VoucherTemplateId == 0)
            {
                await MessageService.Error("Vui lòng chọn mẫu voucher!");
                return;
            }
            if (string.IsNullOrEmpty(voucherRequest.AccountId))
            {
                await MessageService.Error("Vui lòng chọn thành viên!");
                return;
            }

            isCreatingVoucher = true;
            try
            {
                var expiry = voucherRequest.Expiry.HasValue ? DateTimeOffset.Parse(voucherRequest.Expiry.ToString()) : (DateTimeOffset?)null;
                var createdVoucher = await VouchersService.CreateVoucherAsync(
                    voucherRequest.VoucherTemplateId,
                    voucherRequest.AccountId,
                    expiry
                );
                if (createdVoucher != null)
                {
                    voucherRequest = new VoucherRequest();
                    await MessageService.Success($"Đã tặng voucher '{createdVoucher.Code}' thành công!");
                }
                else
                {
                    await MessageService.Error("Không thể tặng voucher!");
                }
            }
            catch (HttpRequestException ex)
            {
                await MessageService.Error($"Lỗi khi tặng voucher: {ex.Message} (Status: {ex.StatusCode})");
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi không xác định: {ex.Message}");
            }
            finally
            {
                isCreatingVoucher = false;
                StateHasChanged();
            }
        }
    }
}
