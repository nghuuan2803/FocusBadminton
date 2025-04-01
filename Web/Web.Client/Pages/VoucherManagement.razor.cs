using AntDesign;
using Shared.Members;
using Shared.Vouchers;

namespace Web.Client.Pages
{
    public partial class VoucherManagement
    {
        [Inject] VouchersService VouchersService { get; set; }
        [Inject]
        MembersService MembersService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IMessageService MessageService { get; set; }
        [Inject] IConfirmService ConfirmService { get; set; }

        private VoucherTemplateDTO? editingTemplate = null;
        private VoucherTemplateDTO newTemplate = new VoucherTemplateDTO

        {
            DiscountType = DiscountType.Percent,
            Value = 0,
            MaximumValue = 0,
            Duration = 30
        };

        IEnumerable<VoucherTemplateDTO> _selectedRows = [];


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
        private async Task OnTemplateSearchChange(ChangeEventArgs e)
        {
            templateSearch = e.Value?.ToString() ?? string.Empty;
            templatePageNumber = 1;
            await LoadTemplates();
        }

        private async Task OnMemberSearchChange(ChangeEventArgs e)
        {
            memberSearch = e.Value?.ToString() ?? string.Empty;
            memberPageNumber = 1;
            await LoadMembers();
        }

        private async Task HandleTemplateTableChange(PaginationEventArgs args)
        {
            templatePageNumber = args.Page;
            templatePageSize = args.PageSize;
            await LoadTemplates();
        }

        private async Task HandleMemberTableChange(PaginationEventArgs args)
        {
            memberPageNumber = args.Page;
            memberPageSize = args.PageSize;
            await LoadMembers();
        }

        private async Task LoadTemplates()
        {
            try
            {
                var result = await VouchersService.GetVoucherTemplatesAsync(templateSearch, templatePageNumber, templatePageSize);
                if (result != null)
                {
                    voucherTemplates = result;
                    templateTotal = result.Count; // Cần backend trả về tổng số thực tế
                }
                else
                {
                    voucherTemplates = new List<VoucherTemplateDTO>();
                    templateTotal = 0;
                }
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi khi tải danh sách mẫu: {ex.Message}");
            }
            StateHasChanged();
        }

        private async Task LoadMembers()
        {
            try
            {
                var result = await MembersService.GetMembersAsync(memberSearch, memberPageNumber, memberPageSize);
                if (result != null)
                {
                    members = result;
                    memberTotal = result.Count; // Cần backend trả về tổng số thực tế
                }
                else
                {
                    members = new List<MemberDTO>();
                    memberTotal = 0;
                }
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi khi tải danh sách thành viên: {ex.Message}");
            }
            StateHasChanged();
        }

        private void ShowCreateTemplateModal()
        {
            newTemplate = new VoucherTemplateDTO
            {
                DiscountType = DiscountType.Percent,
                Value = 0,
                MaximumValue = 0,
                Duration = 30
            };
            editingTemplate = null;
            isTemplateModalVisible = true;
        }

        private void ShowEditTemplateModal(VoucherTemplateDTO template)
        {
            newTemplate = new VoucherTemplateDTO
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                DiscountType = template.DiscountType,
                Value = template.Value,
                MaximumValue = template.MaximumValue,
                Duration = template.Duration
            };
            editingTemplate = template;
            isTemplateModalVisible = true;
        }

        private void HideTemplateModal()
        {
            isTemplateModalVisible = false;
        }

        private async Task HandleTemplateSubmit()
        {
            if (editingTemplate == null)
            {
                await HandleCreateTemplate();
            }
            else
            {
                await HandleUpdateTemplate();
            }
            await LoadTemplates();
            isTemplateModalVisible = false;
        }

        private async Task DeleteTemplate(int id)
        {
            var confirmed = await ConfirmService.Show("Bạn có chắc muốn xóa mẫu voucher này?", "Xác nhận xóa", ConfirmButtons.YesNo);
            if (confirmed == ConfirmResult.Yes)
            {
                try
                {
                    var success = await VouchersService.DeleteVoucherTemplateAsync(id);
                    if (success)
                    {
                        await MessageService.Success("Xóa mẫu voucher thành công!");
                        await LoadTemplates();
                    }
                    else
                    {
                        await MessageService.Error("Không thể xóa mẫu voucher!");
                    }
                }
                catch (Exception ex)
                {
                    await MessageService.Error($"Lỗi: {ex.Message}");
                }
            }
        }

        private void ShowGiftVoucherModal()
        {
            voucherRequest = new VoucherRequest();
            isGiftModalVisible = true;
        }

        private void HideGiftVoucherModal()
        {
            isGiftModalVisible = false;
        }

        private async Task HandleGiftVoucher()
        {
            if (voucherRequest.VoucherTemplateId == 0)
            {
                await MessageService.Error("Vui lòng chọn mẫu voucher!");
                return;
            }

            isCreatingVoucher = true;
            try
            {
                var expiry = voucherRequest.Expiry.HasValue ? DateTimeOffset.Parse(voucherRequest.Expiry.ToString()) : (DateTimeOffset?)null;
                foreach (var accountId in selectedMemberIds)
                {
                    var createdVoucher = await VouchersService.CreateVoucherAsync(voucherRequest.VoucherTemplateId, accountId, expiry);
                    if (createdVoucher != null)
                    {
                        await MessageService.Success($"Đã tặng voucher '{createdVoucher.Code}' cho thành viên!");
                    }
                }
                isGiftModalVisible = false;
                selectedMemberIds.Clear();
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi: {ex.Message}");
            }
            finally
            {
                isCreatingVoucher = false;
                StateHasChanged();
            }
        }

        private async Task GiftToAllMembers()
        {
            selectedMemberIds = new HashSet<string>(members.Select(m => m.AccountId ?? "none"));
            ShowGiftVoucherModal();
        }

        private class VoucherRequest
        {
            public int VoucherTemplateId { get; set; }
            public string AccountId { get; set; } = string.Empty;
            public DateTime? Expiry { get; set; }
        }

    }
}
