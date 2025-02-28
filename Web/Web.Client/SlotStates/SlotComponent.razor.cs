using Microsoft.AspNetCore.Components;
using static Shared.Schedules.ScheduleDTO;
using Web.Client.ApiServices;

namespace Web.Client.SlotStates
{
    public partial class SlotComponent
    {
        [Parameter] public int CourtId { get; set; }        // ID của sân
        [Parameter] public int TimeSlotId { get; set; }     // ID của khung giờ
        [Parameter] public string? HeldBy { get; set; }     // Người giữ chỗ (nếu có)
        [Parameter] public ScheduleStatus InitialStatus { get; set; } // Trạng thái ban đầu

        [Inject] private SlotService SlotService { get; set; } // Service để gọi API

        private ISlotState _state;

        protected override void OnInitialized()
        {
            _state = GetStateFromStatus(InitialStatus); // Khởi tạo trạng thái
        }

        // Chuyển đổi từ ScheduleStatus sang ISlotState
        private ISlotState GetStateFromStatus(ScheduleStatus status)
        {
            return status switch
            {
                ScheduleStatus.Available => new AvailableState(),
                ScheduleStatus.Pending => new PendingState(),
                ScheduleStatus.Booked => new BookedState(),
                ScheduleStatus.Holding => new HoldingState(),
                ScheduleStatus.Paused => new PausedState(),
                ScheduleStatus.TimeOut => new TimeOutState(),
                ScheduleStatus.Blocked => new BlockedState(),
                ScheduleStatus.Completed => new CompletedState(),
                _ => throw new ArgumentException("Trạng thái không hợp lệ")
            };
        }

        // Xử lý click
        private async Task OnSlotClick()
        {
            await _state.HandleClick(this);
        }

        // Cập nhật trạng thái từ SignalR
        public void HandleRealtimeSignal(ScheduleStatus newStatus, string? newHeldBy)
        {
            _state = GetStateFromStatus(newStatus);
            HeldBy = newHeldBy;
            StateHasChanged(); // Cập nhật UI
        }
        public void TransitionTo(ISlotState newState)
        {
            _state = newState;
            StateHasChanged(); // Cập nhật UI
        }

        // Hành động giữ chỗ
        public async Task HoldSlotAsync()
        {
            var holdRequest = new HoldSlotRequest();
            //
            // Điền thông tin cần thiết vào holdRequest
            //
            int holdId = await SlotService.HoldAsync(holdRequest);
            if (holdId > 0)
            {
                HeldBy = "admin"; // Hiển thị người giữ chỗ
                _state = new PendingState(); // Chuyển sang trạng thái chờ duyệt
                StateHasChanged(); // Cập nhật UI
            }
            // Sau khi giữ chỗ thành công, có thể cập nhật trạng thái qua SignalR
        }
        public async Task CancelHoldAsync()
        {
            await SlotService.ReleaseAsync(CourtId, HeldBy);
            HeldBy = null;
            _state = new AvailableState(); // Chuyển về trạng thái trống
            StateHasChanged(); // Cập nhật UI
        }

        // Hành động xem chi tiết đơn đặt
        public async Task ViewBookingDetailsAsync()
        {
            // Logic mở modal hoặc chuyển hướng để xem chi tiết
            await Task.CompletedTask; // Thay bằng logic thực tế
        }

        public async Task BlockSlotAsync()
        {
            //var blockRequest = new BlockSlotRequest();
            //
            // Điền thông tin cần thiết vào blockRequest
            //
            //await SlotService.BlockAsync(blockRequest);
            _state = new BlockedState(); // Chuyển sang trạng thái bị chặn
            StateHasChanged(); // Cập nhật UI
        }

        public async Task UnblockSlotAsync()
        {
            //await SlotService.UnblockAsync(CourtId, TimeSlotId, BeginDate, EndDate);
            _state = new AvailableState(); // Chuyển về trạng thái trống
            StateHasChanged(); // Cập nhật UI
        }
    }
}
