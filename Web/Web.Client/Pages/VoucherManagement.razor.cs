using AntDesign;
using Shared.Members;
using Shared.Vouchers;

namespace Web.Client.Pages
{
    public partial class VoucherManagement
    {
        [Inject] private VouchersService vouchersService { get; set; } = null!;
        [Inject] private MembersService membersService { get; set; } = null!;
        [Inject] private IMessageService messageService { get; set; } = null!;

        protected VoucherTemplateDTO newTemplate = new VoucherTemplateDTO
        {
            DiscountType = DiscountType.Percent,
            Value = 0,
            MaximumValue = 0,
            Duration = 30
        };
        protected List<VoucherTemplateDTO> voucherTemplates = new();
        protected List<MemberDTO> members = new();
        protected VoucherRequest voucherRequest = new VoucherRequest();
        protected bool isCreatingTemplate = false;
        protected bool isCreatingVoucher = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadTemplates();
            await LoadMembers();
        }

        protected async Task HandleCreateTemplate()
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

        protected async Task HandleUpdateTemplate()
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
                var updatedTemplate = await VouchersService.UpdateVoucherTemplateAsync(newTemplate);
                if (updatedTemplate != null)
                {
                    var index = voucherTemplates.FindIndex(t => t.Id == updatedTemplate.Id);
                    if (index != -1)
                    {
                        voucherTemplates[index] = updatedTemplate;
                    }
                    newTemplate = new VoucherTemplateDTO
                    {
                        DiscountType = DiscountType.Percent,
                        Value = 0,
                        MaximumValue = 0,
                        Duration = 30
                    };
                    await MessageService.Success("Cập nhật mẫu voucher thành công!");
                }
                else
                {
                    await MessageService.Error("Không thể cập nhật mẫu voucher!");
                }
            }
            catch (HttpRequestException ex)
            {
                await MessageService.Error($"Lỗi khi cập nhật mẫu: {ex.Message} (Status: {ex.StatusCode})");
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

        protected async Task HandleCreateVoucher()
        {
            await Task.CompletedTask; // Không cần vì đã dùng HandleGiftVoucher
        }

        protected class VoucherRequest
        {
            public int VoucherTemplateId { get; set; }
            public string AccountId { get; set; } = string.Empty;
            public DateTime? Expiry { get; set; }
        }
    }
}