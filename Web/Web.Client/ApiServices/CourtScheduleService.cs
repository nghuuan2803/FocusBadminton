using System.Net.Http.Json;

namespace Web.Client.ApiServices
{
    public class CourtScheduleService
    {
        private readonly HttpClient _httpClient;

        public CourtScheduleService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ScheduleDTO>> GetFacilitySchedules(int facilityId, DateTime date)
        {
            var queryString = $"?facilityId={facilityId}&date={date:yyyy-MM-dd}";
            var data =  await _httpClient.GetFromJsonAsync<List<ScheduleDTO>>($"api/schedules/facility/{queryString}");
            return data!;
        }
        public async Task<List<ScheduleDTO>> GetCourtSchedules(int CourtId, DateTime date)
        {
            var queryString = $"?CourtId={CourtId}&date={date:yyyy-MM-dd}";
            var data = await _httpClient.GetFromJsonAsync<List<ScheduleDTO>>($"api/schedules/court/{queryString}");
            return data!;
        }
        public async Task<List<CourtScheduleDTO>> GetCourtSchedulesInRange(int CourtId, DateTime startDate, DateTime endDate)
        {
            var queryString = $"?CourtId={CourtId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            var data = await _httpClient.GetFromJsonAsync<List<CourtScheduleDTO>>($"api/schedules/court-range/{queryString}");
            return data!;
        }
    }
}
