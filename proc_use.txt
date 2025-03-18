exec GetBookingScheduleForAdmin 1, '20250303';
exec GetBookingScheduleForCourt 1, '20250303';



exec CheckBookingHold 3,1,2,'20250201','20250403', 'Tuesday';
exec CheckBookingHold 1,1,2,'20250201','20250330', 'Monday';
exec CheckBookingHold 3,1,3,'20250302',null, 'Sunday';
exec CheckBookingHold 1,1,1,'20250304',null, 'Monday';
exec CheckBookingHold 4,1,3,'20250302',null, 'Sunday';
exec CheckBookingHold 4,1,3,'20250304',null, 'Sunday';

exec CheckBookingHold 1,1,1,'20250224',null, 'Monday';

EXEC CheckBookingHold 
    @CourtId = 4, 
    @TimeSlotId = 1, 
    @BookingType = 1, 
    @BeginAt = '2025-03-04', 
    @EndAt = NULL, 
    @DayOfWeek = NULL;

SELECT * 
FROM BookingHolds
WHERE CourtId = 3
  AND TimeSlotId = 1
  AND DayOfWeek = 'Tuesday'
  AND ExpiresAt > SYSDATETIMEOFFSET();

SELECT bd.*, b.*
FROM BookingDetails bd
JOIN Bookings b ON bd.BookingId = b.Id
WHERE bd.CourtId = 3
  AND bd.TimeSlotId = 1
  AND bd.DayOfWeek = 'Tuesday'
  AND b.Status NOT IN (4, 5, 6);

--lấy lịch của 1 sân từ ngày đến ngày
EXEC GetBookingScheduleForCourtInRange 
    @CourtId = 1, 
    @StartDate = '2025-03-03', 
    @EndDate = '2025-03-03';

EXEC GetBookingScheduleForCourt
    @CourtId = 1, 
    @Date = '2025-03-03';