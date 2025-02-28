using Microsoft.AspNetCore.Components;
using Shared.Schedules;
using Web.Client.ApiServices;

namespace Web.Client.Pages
{
    public partial class Court
    {
        [Inject] 
        public CourtScheduleService? CourtScheduleService { get; set; }
        private IEnumerable<ScheduleDTO> schedules = [];
        public DateTime Date { get; set; } = DateTime.Now.Date;
        private bool isLoading = false;

        [Parameter]
        public int CourtId { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await LoadSchedules();
        }
        protected override async Task OnParametersSetAsync()
        {
            await LoadSchedules();
        }
        private async Task LoadSchedules()
        {
            isLoading = true;
            try
            {
                schedules = await CourtScheduleService!.GetCourtSchedules(CourtId, Date) ?? [];
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
            Date = DateTime.Now.Date;
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
