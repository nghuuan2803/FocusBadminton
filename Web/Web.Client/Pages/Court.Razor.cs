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

        private string GetStatusText(ScheduleStatus status) => status switch
        {
            ScheduleStatus.Available => "Trống",
            ScheduleStatus.TimeOut => "Quá giờ",
            ScheduleStatus.Holding => "Đang giữ",
            ScheduleStatus.Booked => "Đã đặt",
            ScheduleStatus.Paused => "Tạm ngưng",
            _ => "Không xác định"
        };

        private string GetStatusColor(ScheduleStatus status) => status switch
        {
            ScheduleStatus.Available => "green",
            ScheduleStatus.TimeOut => "gray",
            ScheduleStatus.Holding => "orange",
            ScheduleStatus.Booked => "blue",
            ScheduleStatus.Paused => "red",
            _ => "default"
        };
    }
}
