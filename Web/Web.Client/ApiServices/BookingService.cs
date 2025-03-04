using Shared.Bookings;
using System.Net.Http.Json;

namespace Web.Client.ApiServices
{
    public class BookingService
    {
        private string user = "admin";
        private readonly HttpClient _httpClient;
        public BookingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<BookingDTO> GetBookingAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<BookingDTO>($"api/bookings/{id}");
        }
        public async Task<BookingDTO> ApproveBooking(int bookingId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bookings/approve", new ApproveBookingRequest(bookingId,user));
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<BookingDTO>();
                return data;
            }
            return null;
        }
        public async Task<BookingDTO> RejectBooking(int bookingId)
        {
            var response = await _httpClient.PostAsJsonAsync("api/bookings/reject", new RejectBookingRequest(bookingId, user));
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<BookingDTO>();
                return data;
            }
            return null;
        }
    }
}
