global using Shared.Statistic;

namespace Web.Client.ApiServices
{
    public class TimeSlotService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        public TimeSlotService(HttpClient httpClient, ILocalStorageService localStorageService)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
        }
        public async Task<IEnumerable<TimeSlotDTO>> GetAll()
        {
            var response = await _httpClient.GetAsync("api/TimeSlots");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<TimeSlotDTO>>();
            return result!;
        }
        public async Task<TimeSlotStatisticDTO> GetStatictis(int year, int month, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            var request = new
            {
                Year = year,
                Month = month,
                StartDate = startDate,
                EndDate = endDate
            };
            var response = await _httpClient.PostAsJsonAsync("api/TimeSlots/statictis", request);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<TimeSlotStatisticDTO>();
            return result!;
        }
        public async Task Update(TimeSlotDTO timeSlot)
        {
            var response = await _httpClient.PostAsJsonAsync($"api/TimeSlots/update", new {timeSlot.Id, timeSlot.Price});
            response.EnsureSuccessStatusCode();
        }
    }
}
