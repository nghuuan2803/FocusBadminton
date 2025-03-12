using Shared.Enums;
using System.Net.Http.Json;

namespace Web.Client.ApiServices
{
    public class SlotService
    {
        private readonly HttpClient _httpClient;

        public SlotService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> HoldAsync(HoldSlotRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/slot/hold", request);
            if(!response.IsSuccessStatusCode)
            {
                return 0;
            }
            return await response.Content.ReadFromJsonAsync<int>();
        }
        public async Task<bool> ReleaseAsync(ReleaseSlotRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/slot/release",request);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            return true;
        }
        public async Task<bool> BlockAsyncAsync(int courtId,int timeSlotId, DateTime date)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UnblockAsyncAsync(int courtId, int timeSlotId, DateTime date)
        {
            throw new NotImplementedException();
        }
    }   
}
