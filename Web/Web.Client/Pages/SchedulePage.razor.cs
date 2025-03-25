using Microsoft.Extensions.Configuration;

namespace Web.Client.Pages
{
    public partial class SchedulePage
    {
        [Inject] private CourtScheduleService CourtScheduleService { get; set; } = null!;
        [Inject] private ILogger<ScheduleDTO> Logger { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IConfiguration Configuration { get; set; } = null!;

        private SlotEventHelper _realtimeHelper = null!;
        public DateTime Date { get; set; } = DateTime.Today;
        private List<ScheduleDTO> schedules = [];
        private List<string> courts = [];
        private List<TimeSlotDTO> timeSlots = [];
        private Dictionary<string, SlotComponent> slotComponentRefs = new();
        private bool isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            var config = Configuration.GetSection("ApiSettings");
            string baseUrl = config["BaseAddress"] ?? NavigationManager.BaseUri;
            _realtimeHelper = new SlotEventHelper($"{baseUrl}slotHub");
            _realtimeHelper.OnSlotHeld += HandleSlotHeld;
            _realtimeHelper.OnSlotReleased += HandleSlotReleased;
            _realtimeHelper.OnBookingCreated += HandleBookingCreated;

            await _realtimeHelper.StartAsync();
            await LoadSchedules();
        }

        private async Task LoadSchedules()
        {
            isLoading = true;
            try
            {
                Logger.LogInformation($"Loading schedules for FacilityId: 1, Date: {Date:yyyy-MM-dd}");
                schedules = await CourtScheduleService.GetFacilitySchedules(1, Date) ?? [];
                courts = schedules.Select(s => s.CourtName!).Distinct().OrderBy(c => c).ToList();
                timeSlots = schedules.Select(s => new TimeSlotDTO { Id = s.TimeSlotId, StartTime = s.StartTime, EndTime = s.EndTime })
                                    .DistinctBy(t => t.Id)
                                    .OrderBy(t => t.StartTime)
                                    .ToList();
                Logger.LogInformation($"Loaded {schedules.Count} schedules, {courts.Count} courts, {timeSlots.Count} time slots.");

                slotComponentRefs.Clear(); // Reset danh sách tham chiếu khi reload
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading schedules: {ex.Message}");
                schedules = [];
                courts = [];
                timeSlots = [];
            }
            finally
            {
                isLoading = false;
            }
            StateHasChanged();
        }

        private void HandleSlotUpdated(SlotComponent updatedSlot)
        {
            var key = $"{updatedSlot.CourtId}_{updatedSlot.TimeSlotId}";
            if (!slotComponentRefs.ContainsKey(key))
            {
                slotComponentRefs[key] = updatedSlot;
            }
            StateHasChanged();
        }

        private void HandleSlotHeld(object payload)
        {
            Logger.LogInformation("Slot held: Payload = {@Payload}", payload);
            try
            {
                var payloadString = payload.ToString();
                var slotPayload = JsonConvert.DeserializeObject<SlotPayload>(payloadString);
                if (slotPayload == null)
                {
                    Logger.LogWarning("Failed to deserialize SlotHeld payload.");
                    return;
                }

                // Kiểm tra xem ngày hiện tại có khớp với payload không
                if (!IsDateMatching(slotPayload, Date)) return;

                var slot = schedules.FirstOrDefault(s => s.CourtId == slotPayload.CourtId && s.TimeSlotId == slotPayload.TimeSlotId);
                if (slot != null)
                {
                    slot.Status = ScheduleStatus.Holding;
                    slot.HeldBy = slotPayload.HeldBy;
                    slot.HoldId = slotPayload.HoldSlotId;
                }
                else
                {
                    var courtName = schedules.FirstOrDefault(s => s.CourtId == slotPayload.CourtId)?.CourtName;
                    var timeSlot = timeSlots.FirstOrDefault(t => t.Id == slotPayload.TimeSlotId);
                    if (courtName != null && timeSlot != null)
                    {
                        slot = new ScheduleDTO
                        {
                            CourtId = slotPayload.CourtId,
                            CourtName = courtName,
                            TimeSlotId = slotPayload.TimeSlotId,
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime,
                            Status = ScheduleStatus.Holding,
                            HeldBy = slotPayload.HeldBy,
                            HoldId = slotPayload.HoldSlotId
                        };
                        schedules.Add(slot);
                        courts = schedules.Select(s => s.CourtName!).Distinct().OrderBy(c => c).ToList();
                    }
                }

                var key = $"{slotPayload.CourtId}_{slotPayload.TimeSlotId}";
                if (slotComponentRefs.TryGetValue(key, out var slotComponent))
                {
                    slotComponent.HandleRealtimeSignal(ScheduleStatus.Holding, slotPayload.HoldSlotId, slotPayload.HeldBy);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling SlotHeld: {Message}", ex.Message);
            }
        }

        private void HandleSlotReleased(object payload)
        {
            Logger.LogInformation("SignalR - Slot released: {@Payload}", payload);
            try
            {
                string payloadString = payload.ToString();
                var slotPayload = JsonConvert.DeserializeObject<SlotPayload>(payloadString);
                if (slotPayload == null)
                {
                    Logger.LogWarning("Failed to deserialize SlotReleased payload.");
                    return;
                }

                // Kiểm tra xem ngày hiện tại có khớp với payload không
                if (!IsDateMatching(slotPayload, Date)) return;

                var slot = schedules.FirstOrDefault(s => s.CourtId == slotPayload.CourtId && s.TimeSlotId == slotPayload.TimeSlotId);
                if (slot != null)
                {
                    slot.Status = ScheduleStatus.Available;
                    slot.HeldBy = null;
                    slot.HoldId = null;
                }

                var key = $"{slotPayload.CourtId}_{slotPayload.TimeSlotId}";
                if (slotComponentRefs.TryGetValue(key, out var slotComponent))
                {
                    slotComponent.HandleRealtimeSignal(ScheduleStatus.Available, -1, null);
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling SlotReleased: {Message}", ex.Message);
            }
        }

        private void HandleBookingCreated(object payload)
        {
            Logger.LogInformation("Booking created: Payload = {@Payload}", payload);
            try
            {
                string payloadString = payload.ToString();
                var bookingPayload = JsonConvert.DeserializeObject<BookingPayload>(payloadString);
                if (bookingPayload == null)
                {
                    Logger.LogWarning("Failed to deserialize BookingCreated payload.");
                    return;
                }

                foreach (var detail in bookingPayload.Details)
                {
                    // Kiểm tra xem ngày hiện tại có khớp với payload không
                    if (!IsDateMatching(new SlotPayload
                    {
                        BookingType = bookingPayload.BookingType,
                        BeginAt = detail.BeginAt,
                        EndAt = detail.EndAt,
                        DayOfWeek = detail.DayOfWeek
                    }, Date)) continue;

                    var slot = schedules.FirstOrDefault(s => s.CourtId == detail.CourtId && s.TimeSlotId == detail.TimeSlotId);
                    var newStatus = bookingPayload.Status switch
                    {
                        1 => ScheduleStatus.Pending,
                        2 => ScheduleStatus.Booked,
                        3 => ScheduleStatus.Paused,
                        4 => ScheduleStatus.Completed,
                        _ => ScheduleStatus.Available
                    };

                    if (slot != null)
                    {
                        slot.Status = newStatus;
                        slot.HeldBy = bookingPayload.BookBy;
                        slot.BookingId = bookingPayload.BookingId;
                    }
                    else
                    {
                        var courtName = schedules.FirstOrDefault(s => s.CourtId == detail.CourtId)?.CourtName;
                        var timeSlot = timeSlots.FirstOrDefault(t => t.Id == detail.TimeSlotId);
                        if (courtName != null && timeSlot != null)
                        {
                            slot = new ScheduleDTO
                            {
                                CourtId = detail.CourtId,
                                CourtName = courtName,
                                TimeSlotId = detail.TimeSlotId,
                                StartTime = timeSlot.StartTime,
                                EndTime = timeSlot.EndTime,
                                Status = newStatus,
                                HeldBy = bookingPayload.BookBy,
                                BookingId = bookingPayload.BookingId,
                            };
                            schedules.Add(slot);
                            courts = schedules.Select(s => s.CourtName!).Distinct().OrderBy(c => c).ToList();
                        }
                    }

                    var key = $"{detail.CourtId}_{detail.TimeSlotId}";
                    if (slotComponentRefs.TryGetValue(key, out var slotComponent))
                    {
                        slotComponent.HandleRealtimeSignal(newStatus, -1, bookingPayload.BookBy);
                    }
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error handling BookingCreated: {Message}", ex.Message);
            }
        }

        private async Task HandleFilter()
        {
            await LoadSchedules();
        }

        private async Task ResetFilter()
        {
            Date = DateTime.Today;
            schedules = [];
            await LoadSchedules();
        }

        public async ValueTask DisposeAsync()
        {
            if (_realtimeHelper != null)
            {
                await _realtimeHelper.DisposeAsync();
            }
        }
        private bool IsDateMatching(SlotPayload payload, DateTime currentDate)
        {
            var beginDate = payload.BeginAt?.Date ?? DateTime.Today;
            var endDate = payload.EndAt?.Date ?? (payload.BookingType == 3 ? DateTime.MaxValue : currentDate);

            switch (payload.BookingType)
            {
                case 1: // InDay
                    return currentDate.Date == beginDate;

                case 2: // Fixed
                    return currentDate >= beginDate && currentDate <= endDate &&
                           (string.IsNullOrEmpty(payload.DayOfWeek) || payload.DayOfWeek == currentDate.DayOfWeek.ToString());

                case 3: // Fixed_Unset_EndDate
                    return currentDate >= beginDate &&
                           (string.IsNullOrEmpty(payload.DayOfWeek) || payload.DayOfWeek == currentDate.DayOfWeek.ToString());

                default:
                    Logger.LogWarning("Unknown BookingType: {BookingType}", payload.BookingType);
                    return false;
            }
        }
    }
}
