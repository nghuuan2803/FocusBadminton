using Shared.Vouchers;
using System.Net.Http.Json;

namespace Web.Client.ApiServices
{
    public class VouchersService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/vouchers";

        public VouchersService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Tạo voucher
        public async Task<VoucherDTO?> CreateVoucherAsync(int voucherTemplateId, string accountId, DateTimeOffset? expiry = null)
        {
            var request = new
            {
                VoucherTemplateId = voucherTemplateId,
                AccountId = accountId,
                Expiry = expiry
            };

            var response = await _httpClient.PostAsJsonAsync(BaseUrl, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VoucherDTO>();
        }

        // Lấy danh sách voucher templates
        public async Task<List<VoucherTemplateDTO>> GetVoucherTemplatesAsync(string? name = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(name)) query.Add($"name={Uri.EscapeDataString(name)}");
            if (pageNumber > 1) query.Add($"pageNumber={pageNumber}");
            if (pageSize != 10) query.Add($"pageSize={pageSize}");

            var url = query.Any() ? $"{BaseUrl}/templates?{string.Join("&", query)}" : $"{BaseUrl}/templates";
            return await _httpClient.GetFromJsonAsync<List<VoucherTemplateDTO>>(url) ?? new List<VoucherTemplateDTO>();
        }

        // Tạo voucher template mới
        public async Task<VoucherTemplateDTO?> CreateVoucherTemplateAsync(VoucherTemplateDTO template)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/templates", template);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VoucherTemplateDTO>();
        }
        public async Task<VoucherTemplateDTO?> UpdateVoucherTemplateAsync(VoucherTemplateDTO template)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/templates/{template.Id}", template);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VoucherTemplateDTO>();
        }
        public async Task<bool> DeleteVoucherTemplateAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/templates/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}