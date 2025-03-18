using Shared.Members;
using System.Net.Http.Json;

namespace Web.Client.ApiServices
{
    public class MembersService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/members"; // Base URL của MembersController

        public MembersService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Gọi API để lấy danh sách members
        public async Task<List<MemberDTO>> GetMembersAsync(string? fullName = null, string? phoneNumber = null)
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(fullName)) query.Add($"fullName={Uri.EscapeDataString(fullName)}");
            if (!string.IsNullOrEmpty(phoneNumber)) query.Add($"phoneNumber={Uri.EscapeDataString(phoneNumber)}");

            var url = query.Any() ? $"{BaseUrl}?{string.Join("&", query)}" : BaseUrl;
            return await _httpClient.GetFromJsonAsync<List<MemberDTO>>(url)
                ?? new List<MemberDTO>();
        }
    }
}