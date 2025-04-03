using Shared.Members;

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
        public async Task<List<MemberDTO>> GetMembersAsync(string? fullName = null, int pageNumber = 1, int pageSize = 10)
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(fullName)) query.Add($"fullName={Uri.EscapeDataString(fullName)}");
            if (pageNumber > 1) query.Add($"pageNumber={pageNumber}");
            if (pageSize != 10) query.Add($"pageSize={pageSize}");

            var url = query.Any() ? $"{BaseUrl}?{string.Join("&", query)}" : BaseUrl;
            return await _httpClient.GetFromJsonAsync<List<MemberDTO>>(url) ?? new List<MemberDTO>();
        }
    }
}