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
        public async Task<List<VoucherTemplateDTO>> GetVoucherTemplatesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<VoucherTemplateDTO>>($"{BaseUrl}/templates")
                ?? new List<VoucherTemplateDTO>();
        }

        // Tạo voucher template mới
        public async Task<VoucherTemplateDTO?> CreateVoucherTemplateAsync(VoucherTemplateDTO template)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/templates", template);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<VoucherTemplateDTO>();
        }
    }
}