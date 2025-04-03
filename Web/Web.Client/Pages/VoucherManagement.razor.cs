using AntDesign;
using Shared.Members;
using Shared.Vouchers;

namespace Web.Client.Pages
{
    public partial class VoucherManagement
    {
        [Inject] VouchersService VouchersService { get; set; }
        [Inject] MembersService MembersService { get; set; }
        [Inject] NavigationManager NavigationManager { get; set; }
        [Inject] IMessageService MessageService { get; set; }
        [Inject] IConfirmService ConfirmService { get; set; }

        private VoucherTemplateDTO newTemplate = new VoucherTemplateDTO
        {
            DiscountType = 1,
            Value = 0,
            MaximumValue = 0,
            Duration = 30
        };

        private VoucherTemplateDTO editTemplate = new VoucherTemplateDTO();
        IEnumerable<VoucherTemplateDTO> _selectedRows = [];

        protected override async Task OnInitializedAsync()
        {
            await LoadTemplates();
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
                    _total = result.Count; // Cần backend trả về tổng số thực tế nếu có phân trang
                }
                else
                {
                    voucherTemplates = new List<VoucherTemplateDTO>();
                    _total = 0;
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
                    memberTotal = result.Count; // Cần backend trả về tổng số thực tế nếu có phân trang
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
                DiscountType = 1,
                Value = 0,
                MaximumValue = 0,
                Duration = 30
            };
            isCreateModalVisible = true;
        }

        private void HideCreateModal()
        {
            isCreateModalVisible = false;
        }

        private void ShowEditTemplateModal(VoucherTemplateDTO template)
        {
            editTemplate = new VoucherTemplateDTO
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                DiscountType = template.DiscountType,
                Value = template.Value,
                MaximumValue = template.MaximumValue,
                Duration = template.Duration
            };
            isEditModalVisible = true;
        }

        private void HideEditModal()
        {
            isEditModalVisible = false;
        }

        protected async Task HandleCreateTemplate()
        {
            if (string.IsNullOrWhiteSpace(newTemplate.Name))
            {
                await MessageService.Error("Tên mẫu không được để trống!");
                MessageService.Destroy();

                return;
            }
            if (newTemplate.Value <= 0)
            {
                await MessageService.Error("Giá trị phải lớn hơn 0!");
                MessageService.Destroy();

                return;
            }
            if (newTemplate.Duration <= 0)
            {
                await MessageService.Error("Thời hạn phải lớn hơn 0!");
                MessageService.Destroy();

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
                        DiscountType = 1,
                        Value = 0,
                        MaximumValue = 0,
                        Duration = 30
                    };
                    await MessageService.Success("Tạo mẫu voucher thành công!");
                    MessageService.Destroy();

                    isCreateModalVisible = false;
                    await LoadTemplates();
                }
                else
                {
                    await MessageService.Error("Không thể tạo mẫu voucher!");
                    MessageService.Destroy();
                }
            }
            catch (HttpRequestException ex)
            {
                await MessageService.Error($"Lỗi khi tạo mẫu: {ex.Message} (Status: {ex.StatusCode})");
                MessageService.Destroy();
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi không xác định: {ex.Message}");
                MessageService.Destroy();
            }
            finally
            {
                isCreatingTemplate = false;
                StateHasChanged();
            }
        }

        protected async Task HandleUpdateTemplate()
        {
            if (string.IsNullOrWhiteSpace(editTemplate.Name))
            {
                await MessageService.Error("Tên mẫu không được để trống!");
                MessageService.Destroy();
                return;
            }
            if (editTemplate.Value <= 0)
            {
                await MessageService.Error("Giá trị phải lớn hơn 0!");
                MessageService.Destroy();
                return;
            }
            if (editTemplate.Duration <= 0)
            {
                await MessageService.Error("Thời hạn phải lớn hơn 0!");
                MessageService.Destroy();
                return;
            }

            isCreatingTemplate = true;
            try
            {
                var updatedTemplate = await VouchersService.UpdateVoucherTemplateAsync(editTemplate);
                if (updatedTemplate != null)
                {
                    var index = voucherTemplates.FindIndex(t => t.Id == updatedTemplate.Id);
                    if (index != -1)
                    {
                        voucherTemplates[index] = updatedTemplate;
                    }
                    await MessageService.Success("Cập nhật mẫu voucher thành công!");
                    MessageService.Destroy();

                    isEditModalVisible = false;
                    await LoadTemplates();
                }
                else
                {
                    await MessageService.Error("Không thể cập nhật mẫu voucher!");
                    MessageService.Destroy();
                }
            }
            catch (HttpRequestException ex)
            {
                await MessageService.Error($"Lỗi khi cập nhật mẫu: {ex.Message} (Status: {ex.StatusCode})");
                MessageService.Destroy();
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi không xác định: {ex.Message}");
                MessageService.Destroy();
            }
            finally
            {
                isCreatingTemplate = false;
                StateHasChanged();
            }
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
                        voucherTemplates.RemoveAll(t => t.Id == id);
                        await MessageService.Success("Xóa mẫu voucher thành công!");
                        MessageService.Destroy();

                        await LoadTemplates();
                    }
                    else
                    {
                        await MessageService.Error("Không thể xóa mẫu voucher!");
                        MessageService.Destroy();
                    }
                }
                catch (Exception ex)
                {
                    await MessageService.Error($"Lỗi: {ex.Message}");
                    MessageService.Destroy();
                }
                finally
                {
                    StateHasChanged();
                }
            }
        }

        private void ShowGiftVoucherModal(VoucherTemplateDTO template)
        {
            selectedTemplate = template;
            voucherRequest = new VoucherRequest { VoucherTemplateId = template.Id };
            _selectedMember = null;
            isGiftModalVisible = true;
        }

        private void HideGiftVoucherModal()
        {
            isGiftModalVisible = false;
            selectedTemplate = null;
        }

        private async Task HandleGiftVoucher()
        {
            if (_selectedMember == null || string.IsNullOrEmpty(_selectedMember.AccountId))
            {
                await MessageService.Error("Vui lòng chọn một thành viên!");
                return;
            }

            var confirmed = await ConfirmService.Show($"Tặng voucher cho {_selectedMember.FullName}?", "Xác nhận", ConfirmButtons.YesNo);
            if (confirmed == ConfirmResult.Yes)
            {
                isCreatingVoucher = true;
                try
                {
                    var expiry = voucherRequest.Expiry.HasValue ? DateTimeOffset.Parse(voucherRequest.Expiry.ToString()) : (DateTimeOffset?)null;
                    var createdVoucher = await VouchersService.CreateVoucherAsync(voucherRequest.VoucherTemplateId, _selectedMember.AccountId, expiry);
                    if (createdVoucher != null)
                    {
                        await MessageService.Success($"Đã tặng voucher '{createdVoucher.Code}' cho {_selectedMember.FullName}!");
                        MessageService.Destroy();
                        isGiftModalVisible = false;
                    }
                    else
                    {
                        await MessageService.Error("Không thể tặng voucher!");
                        MessageService.Destroy();
                    }
                }
                catch (Exception ex)
                {
                    await MessageService.Error($"Lỗi: {ex.Message}");
                    MessageService.Destroy();
                }
                finally
                {
                    isCreatingVoucher = false;
                    StateHasChanged();
                }
            }
        }

        private async Task GiftToAllMembers(VoucherTemplateDTO template)
        {
            var confirmed = await ConfirmService.Show($"Bạn có chắc muốn tặng voucher '{template.Name}' cho tất cả {members.Count} thành viên?", "Xác nhận tặng tất cả", ConfirmButtons.YesNo);
            if (confirmed != ConfirmResult.Yes)
            {
                return;
            }

            isCreatingVoucher = true;
            try
            {
                var expiry = DateTimeOffset.UtcNow.AddDays(template.Duration); // Mặc định dùng Duration
                var validMemberIds = members.Where(m => !string.IsNullOrEmpty(m.AccountId)).Select(m => m.AccountId).ToList();

                if (!validMemberIds.Any())
                {
                    await MessageService.Error("Không có thành viên hợp lệ để tặng voucher!");
                    MessageService.Destroy();

                    return;
                }

                var result = await VouchersService.BulkCreateVouchersAsync(template.Id, validMemberIds, expiry);
                if (result)
                {
                    await MessageService.Success($"Tặng voucher thành công!");
                    MessageService.Destroy();
                }
                else
                {
                    await MessageService.Error("Không thể tặng voucher cho bất kỳ thành viên nào!");
                    MessageService.Destroy();
                }
            }
            catch (Exception ex)
            {
                await MessageService.Error($"Lỗi: {ex.Message}");
                MessageService.Destroy();
            }
            finally
            {
                isCreatingVoucher = false;
                StateHasChanged();
            }
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

        private class VoucherRequest
        {
            public int VoucherTemplateId { get; set; }
            public string AccountId { get; set; } = string.Empty;
            public DateTime? Expiry { get; set; }
        }
    }
}