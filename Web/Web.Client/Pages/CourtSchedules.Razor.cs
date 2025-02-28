using Shared.Schedules;
using Web.Client.ApiServices;

namespace Web.Client.Pages
{
    public partial class CourtSchedules
    {
        public DateTime Date { get; set; } = DateTime.Now.Date;
        private IEnumerable<ScheduleDTO> schedules = new List<ScheduleDTO>();
        private bool isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            await LoadSchedules();
        }

        private async Task LoadSchedules()
        {
            isLoading = true;
            try
            {                
                schedules = await CourtScheduleService.GetFacilitySchedules(1, Date) ?? [];
            }
            catch (Exception)
            {
                schedules = [];
            }
            finally
            {
                isLoading = false;
            }
            StateHasChanged();
        }

        private async Task HandleFilter()
        {
            await LoadSchedules();
        }

        private void ResetFilter()
        {
            schedules = new List<ScheduleDTO>();
            StateHasChanged();
            _ = LoadSchedules(); // Gọi lại để load lịch mới sau khi reset
        }

        private string GetStatusText(ScheduleDTO.ScheduleStatus status) => status switch
        {
            ScheduleDTO.ScheduleStatus.Available => "Trống",
            ScheduleDTO.ScheduleStatus.TimeOut => "Quá giờ",
            ScheduleDTO.ScheduleStatus.Holding => "Đang giữ",
            ScheduleDTO.ScheduleStatus.Booked => "Đã đặt",
            ScheduleDTO.ScheduleStatus.Paused => "Tạm ngưng",
            _ => "Không xác định"
        };

        private string GetStatusColor(ScheduleDTO.ScheduleStatus status) => status switch
        {
            ScheduleDTO.ScheduleStatus.Available => "green",
            ScheduleDTO.ScheduleStatus.TimeOut => "gray",
            ScheduleDTO.ScheduleStatus.Holding => "orange",
            ScheduleDTO.ScheduleStatus.Booked => "blue",
            ScheduleDTO.ScheduleStatus.Paused => "red",
            _ => "default"
        };
    }
}
