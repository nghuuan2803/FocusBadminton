// SlotComponent.razor.cs
using AntDesign;
using Shared.Bookings;

namespace Web.Client.SlotStates
{
    public partial class SlotComponent : IDisposable
    {
        [Parameter] public int CourtId { get; set; }
        [Parameter] public int TimeSlotId { get; set; }
        [Parameter] public TimeSpan StartTime { get; set; }
        [Parameter] public TimeSpan EndTime { get; set; }
        [Parameter] public int? HoldId { get; set; }
        [Parameter] public int? BookingId { get; set; }
        [Parameter] public int? BookingDetailId { get; set; }
        [Parameter] public string? HeldBy { get; set; }
        [Parameter] public ScheduleStatus InitialStatus { get; set; }
        [Parameter] public DateTime Date { get; set; } // Thêm Date để tính DayOfWeek

        [Parameter] public EventCallback<SlotComponent> OnSlotUpdated { get; set; }

        [Inject] private SlotService SlotService { get; set; } = null!;
        [Inject] private MessageService MessageService { get; set; } = null!;
        [Inject] private BookingService BookingService { get; set; } = null!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset EndAt { get; set; }

        private ISlotState _state;
        private int holdId;
        private BookingDTO? booking;
        protected async override Task OnInitializedAsync()
        {
            _state = GetStateFromStatus(InitialStatus);
            holdId = HoldId ?? 0;
            await Task.Delay(10);
            if (BookingId != null)
            {
                booking = await BookingService.GetBookingAsync(BookingId.Value);
            }
        }

        private ISlotState GetStateFromStatus(ScheduleStatus status)
        {

            ISlotState state = status switch
            {
                ScheduleStatus.Available => new AvailableState(),
                ScheduleStatus.Pending => new PendingState(),
                ScheduleStatus.Booked => new BookedState(),
                ScheduleStatus.Holding => new HeldState(),
                ScheduleStatus.Paused => new PausedState(),
                ScheduleStatus.TimeOut => new TimeOutState(),
                ScheduleStatus.Blocked => new BlockedState(),
                ScheduleStatus.Completed => new CompletedState(),
                _ => throw new ArgumentException("Trạng thái không hợp lệ")
            };
            if (state is HeldState && HeldBy == "1")
                state = new HoldingState();
            return state;
        }

        private async Task OnSlotClick()
        {
            await _state.HandleClick(this);
        }

        public void HandleRealtimeSignal(ScheduleStatus newStatus,int holdId, string? newHeldBy)
        {
            this.holdId = holdId;
            HeldBy = newHeldBy;
            _state = GetStateFromStatus(newStatus);
            StateHasChanged();
        }

        public async Task HoldSlotAsync()
        {
            var holdRequest = new HoldSlotRequest
            {
                CourtId = CourtId,
                TimeSlotId = TimeSlotId,
                HoldBy = "1", // Thay bằng thông tin người dùng thực tế
                BookingType = BookingType.InDay,
                BeginAt = new DateTimeOffset(Date),
            };

            this.holdId = await SlotService.HoldAsync(holdRequest);
            if (holdId < 1)
            {
                await MessageService.Error("Không thể chọn!");
                //HeldBy = holdRequest.HoldBy;
                TransitionTo(new HoldingState());
                StateHasChanged();
            }
        }

        public async Task CancelHoldAsync()
        {
            if (holdId < 1)
            {
                return;
            }
            var releaseRequest = new ReleaseSlotRequest
            {
                HoldId = holdId,
                HeldBy = "1"
            };

            bool success = await SlotService.ReleaseAsync(releaseRequest);
            if (success)
            {
                HeldBy = null;
                TransitionTo(new AvailableState());
                StateHasChanged();
            }
            else
            {
                MessageService.Error($"Hold {holdId}");
            }
        }

        public async Task ViewBookingDetailsAsync()
        {
            ShowModal(); // Hiện modal thông tin đặt sân
            //ShowModalWithService(); // Hiện modal thông tin đặt sân
        }

        public async Task BlockSlotAsync()
        {
            await Task.CompletedTask; // Thay bằng logic thực tế
            TransitionTo(new BlockedState());
            await OnSlotUpdated.InvokeAsync(this);
        }

        public async Task UnblockSlotAsync()
        {
            await Task.CompletedTask; // Thay bằng logic thực tế
            TransitionTo(new AvailableState());
            await OnSlotUpdated.InvokeAsync(this);
        }

        public void TransitionTo(ISlotState newState)
        {
            _state = newState;
            StateHasChanged();
        }

        private async Task ApproveBooking()
        {
            if (BookingId != null)
            {
                var result = await BookingService.ApproveBooking((int)BookingId);
                if (result != null)
                {
                    booking = result;
                    TransitionTo(new BookedState());
                    await MessageService.Success("Duyệt thành công", 3);
                }
                else
                {
                    await MessageService.Error("Lỗi", 3);
                }
                StateHasChanged();
            }
        }
        private async Task RejectBooking()
        {
            if (BookingId != null)
            {
                var result = await BookingService.RejectBooking((int)BookingId);
                if (result != null)
                {
                    booking = result;
                    ISlotState newState = Date < DateTime.Now ? new TimeOutState() : new AvailableState();
                    TransitionTo(newState);
                    await MessageService.Success("Đã hủy yêu cầu", 3);
                }
                else
                {
                    await MessageService.Error("Lỗi", 3);
                }
                StateHasChanged();
                await Task.Delay(1000);
            }
        }

        #region not implement

        public void OnHeld(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnReleased(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingCreated(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingCanceled(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingApproved(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingRejected(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingPaused(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingResumed(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnBookingCompleted(object payload)
        {
            throw new NotImplementedException();
        }

        public void OnSlotTimeOut()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // Dọn dẹp nếu cần
        }

        #endregion
    }
}