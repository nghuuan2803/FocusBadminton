using AntDesign;

namespace Web.Client.Pages
{
    public partial class CourtSchedules
    {
        [Parameter] public int CourtId { get; set; }
        [Inject] private CourtScheduleService CourtScheduleService { get; set; } = null!;
        [Inject] private ILogger<CourtScheduleDTO> Logger { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IConfiguration Configuration { get; set; } = null!;

        private SlotEventListener _realtimeHelper = null!;
        private DateTime StartDate { get; set; } = DateTime.Today;
        private DateTime EndDate { get; set; } = DateTime.Today.AddDays(6); // Mặc định là 7 ngày sau
        private List<CourtScheduleDTO> schedules = [];
        private string courtName = "";
        private List<TimeSlotDTO> timeSlots = [];
        private Dictionary<string, SlotComponent> slotComponentRefs = new();
        private bool isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            var config = Configuration.GetSection("ApiSettings");
            string baseUrl = config["BaseAddress"] ?? NavigationManager.BaseUri;
            Console.WriteLine(baseUrl);
            _realtimeHelper = new SlotEventListener($"{baseUrl}slotHub");
            _realtimeHelper.OnSlotHeld += HandleSlotHeld;
            _realtimeHelper.OnSlotReleased += HandleSlotReleased;
            _realtimeHelper.OnBookingCreated += HandleBookingCreated;

            await _realtimeHelper.StartAsync();
            //await LoadSchedules();
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
                Logger.LogInformation($"Loading schedules for CourtId: {CourtId}, StartDate: {StartDate:yyyy-MM-dd}, EndDate: {EndDate:yyyy-MM-dd}");
                schedules = await CourtScheduleService.GetCourtSchedulesInRange(CourtId, StartDate, EndDate) ?? [];
                courtName = schedules.FirstOrDefault()?.CourtName ?? $"Sân {CourtId}";
                timeSlots = schedules.Select(s => new TimeSlotDTO { Id = s.TimeSlotId, StartTime = s.StartTime, EndTime = s.EndTime })
                                    .DistinctBy(t => t.Id)
                                    .OrderBy(t => t.StartTime)
                                    .ToList();
                Logger.LogInformation($"Loaded {schedules.Count} schedules, {timeSlots.Count} time slots.");

                slotComponentRefs.Clear(); // Reset danh sách tham chiếu khi reload
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error loading schedules: {ex.Message}");
                schedules = [];
                courtName = $"Sân {CourtId}";
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
            var scheduleDate = schedules.FirstOrDefault(s => s.CourtId == updatedSlot.CourtId && s.TimeSlotId == updatedSlot.TimeSlotId)?.ScheduleDate ?? DateTime.Today;
            var key = $"{updatedSlot.CourtId}_{updatedSlot.TimeSlotId}_{scheduleDate:yyyyMMdd}";
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

                if (slotPayload.CourtId != CourtId) return; // Chỉ xử lý cho sân hiện tại

                var scheduleDate = slotPayload.BeginAt?.Date ?? DateTime.Today;
                if (scheduleDate < StartDate || scheduleDate > EndDate) return; // Chỉ xử lý trong khoảng thời gian hiển thị

                var slot = schedules.FirstOrDefault(s => s.CourtId == slotPayload.CourtId && s.TimeSlotId == slotPayload.TimeSlotId && s.ScheduleDate == scheduleDate);
                if (slot != null)
                {
                    slot.Status = ScheduleStatus.Holding;
                    slot.HeldBy = slotPayload.HeldBy;
                }
                else
                {
                    var timeSlot = timeSlots.FirstOrDefault(t => t.Id == slotPayload.TimeSlotId);
                    if (timeSlot != null)
                    {
                        schedules.Add(new CourtScheduleDTO
                        {
                            ScheduleDate = scheduleDate,
                            DayOfWeek = scheduleDate.DayOfWeek.ToString(),
                            CourtId = slotPayload.CourtId,
                            CourtName = courtName,
                            TimeSlotId = slotPayload.TimeSlotId,
                            StartTime = timeSlot.StartTime,
                            EndTime = timeSlot.EndTime,
                            Status = ScheduleStatus.Holding,
                            HeldBy = slotPayload.HeldBy,
                            HoldId = slotPayload.HoldSlotId
                        });
                    }
                }

                var key = $"{slotPayload.CourtId}_{slotPayload.TimeSlotId}_{scheduleDate:yyyyMMdd}";
                if (slotComponentRefs.TryGetValue(key, out var slotComponent))
                {
                    slotComponent.HandleRealtimeSignal(ScheduleStatus.Holding, slotPayload.HoldSlotId, slotPayload.HeldBy);
                }
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

                if (slotPayload.CourtId != CourtId) return;

                var scheduleDate = slotPayload.BeginAt?.Date ?? DateTime.Today;
                if (scheduleDate < StartDate || scheduleDate > EndDate) return;

                var slot = schedules.FirstOrDefault(s => s.CourtId == slotPayload.CourtId && s.TimeSlotId == slotPayload.TimeSlotId && s.ScheduleDate == scheduleDate);
                if (slot != null)
                {
                    slot.Status = ScheduleStatus.Available;
                    slot.HeldBy = null;
                    slot.HoldId = null;
                }

                var key = $"{slotPayload.CourtId}_{slotPayload.TimeSlotId}_{scheduleDate:yyyyMMdd}";
                if (slotComponentRefs.TryGetValue(key, out var slotComponent))
                {
                    slotComponent.HandleRealtimeSignal(ScheduleStatus.Available, -1, null);
                }
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
                    if (detail.CourtId != CourtId) continue;

                    var scheduleDate = detail.BeginAt?.Date ?? DateTime.Today;
                    if (scheduleDate < StartDate || scheduleDate > EndDate) continue;

                    bool matchesDay = detail.DayOfWeek == null || detail.DayOfWeek == scheduleDate.DayOfWeek.ToString();
                    bool withinDateRange = (detail.BeginAt.Value.Date <= scheduleDate) &&
                                          (detail.EndAt == null || detail.EndAt.Value.Date >= scheduleDate);

                    if (matchesDay && withinDateRange)
                    {
                        var slot = schedules.FirstOrDefault(s => s.CourtId == detail.CourtId && s.TimeSlotId == detail.TimeSlotId && s.ScheduleDate == scheduleDate);
                        if (slot != null)
                        {
                            var newStatus = bookingPayload.Status switch
                            {
                                1 => ScheduleStatus.Pending,
                                2 => ScheduleStatus.Booked,
                                3 => ScheduleStatus.Paused,
                                4 => ScheduleStatus.Completed,
                                _ => slot.Status
                            };
                            slot.Status = newStatus;
                            slot.HeldBy = bookingPayload.BookBy;
                        }
                        else
                        {
                            var timeSlot = timeSlots.FirstOrDefault(t => t.Id == detail.TimeSlotId);
                            if (timeSlot != null)
                            {
                                schedules.Add(new CourtScheduleDTO
                                {
                                    ScheduleDate = scheduleDate,
                                    DayOfWeek = scheduleDate.DayOfWeek.ToString(),
                                    CourtId = detail.CourtId,
                                    CourtName = courtName,
                                    TimeSlotId = detail.TimeSlotId,
                                    StartTime = timeSlot.StartTime,
                                    EndTime = timeSlot.EndTime,
                                    Status = bookingPayload.Status switch
                                    {
                                        1 => ScheduleStatus.Pending,
                                        2 => ScheduleStatus.Booked,
                                        3 => ScheduleStatus.Paused,
                                        4 => ScheduleStatus.Completed,
                                        _ => ScheduleStatus.Available
                                    },
                                    HeldBy = bookingPayload.BookBy
                                });
                            }
                        }

                        var key = $"{detail.CourtId}_{detail.TimeSlotId}_{scheduleDate:yyyyMMdd}";
                        if (slotComponentRefs.TryGetValue(key, out var slotComponent))
                        {
                            var newStatus = bookingPayload.Status switch
                            {
                                1 => ScheduleStatus.Pending,
                                2 => ScheduleStatus.Booked,
                                3 => ScheduleStatus.Paused,
                                4 => ScheduleStatus.Completed,
                                _ => slotComponent.InitialStatus
                            };
                            slotComponent.HandleRealtimeSignal(newStatus, -1, null);
                        }
                    }
                }
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
            StartDate = DateTime.Today;
            EndDate = DateTime.Today.AddDays(7);
            schedules = [];
            await LoadSchedules();
            await MessageService.Error("Lỗi", 1);
            MessageService.Destroy();
        }

        public async ValueTask DisposeAsync()
        {
            if (_realtimeHelper != null)
            {
                await _realtimeHelper.DisposeAsync();
            }
        }
    }
}