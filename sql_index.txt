CREATE NONCLUSTERED INDEX IX_TimeSlots_IsApplied ON TimeSlots (IsApplied) INCLUDE (Id, StartTime, EndTime);
CREATE NONCLUSTERED INDEX IX_BookingHolds_CourtId_TimeSlotId_ExpiresAt
ON BookingHolds (CourtId, TimeSlotId, ExpiresAt)
INCLUDE (BookingType, BeginAt, EndAt, DayOfWeek, HeldBy);
CREATE NONCLUSTERED INDEX IX_BookingDetails_CourtId_TimeSlotId_BeginAt_EndAt
ON BookingDetails (CourtId, TimeSlotId, BeginAt, EndAt)
INCLUDE (BookingId, DayOfWeek);
CREATE NONCLUSTERED INDEX IX_Bookings_Status_Type
ON Bookings (Status, Type)
INCLUDE (Id);