go
CREATE OR ALTER PROCEDURE CheckBookingHold
    @CourtId INT,
    @TimeSlotId INT,
    @BookingType INT,
    @BeginAt DATETIMEOFFSET,
    @EndAt DATETIMEOFFSET = NULL,
    @DayOfWeek VARCHAR(10) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentDateTime DATETIMEOFFSET = SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time';
    DECLARE @BeginAtLocal DATETIMEOFFSET = @BeginAt AT TIME ZONE 'SE Asia Standard Time';
    DECLARE @EndAtLocal DATETIMEOFFSET = @EndAt AT TIME ZONE 'SE Asia Standard Time';

    IF @BeginAtLocal IS NULL OR @BeginAtLocal < @CurrentDateTime
    BEGIN
        SELECT 0 AS result, N'Không thể chọn lịch trong quá khứ' AS Message;
        RETURN;
    END

    DECLARE @EffectiveDayOfWeek VARCHAR(10) = @DayOfWeek;
    IF @DayOfWeek IS NULL
    BEGIN
        SET @EffectiveDayOfWeek = DATENAME(WEEKDAY, @BeginAtLocal);
    END

    IF @BookingType IN (2, 3) AND @EffectiveDayOfWeek IS NULL
    BEGIN
        SELECT 0 AS result, N'Đối với đặt cố định, DayOfWeek không được NULL.' AS Message;
        RETURN;
    END

    -- Tạo danh sách ngày thực tế cho request
    DECLARE @RequestDates TABLE (CheckDate DATE);
    IF @BookingType = 1
    BEGIN
        INSERT INTO @RequestDates (CheckDate)
        VALUES (CAST(@BeginAtLocal AS DATE));
    END
    ELSE IF @BookingType = 2
    BEGIN
        WITH DateRange AS (
            SELECT CAST(@BeginAtLocal AS DATE) AS CheckDate
            UNION ALL
            SELECT DATEADD(DAY, 1, CheckDate)
            FROM DateRange
            WHERE CheckDate < CAST(@EndAtLocal AS DATE)
        )
        INSERT INTO @RequestDates (CheckDate)
        SELECT CheckDate
        FROM DateRange
        WHERE DATENAME(WEEKDAY, CheckDate) = @EffectiveDayOfWeek;
    END
    ELSE IF @BookingType = 3
    BEGIN
        DECLARE @FirstMatchingDate DATE = CAST(@BeginAtLocal AS DATE);
        WHILE DATENAME(WEEKDAY, @FirstMatchingDate) != @EffectiveDayOfWeek
        BEGIN
            SET @FirstMatchingDate = DATEADD(DAY, 1, @FirstMatchingDate);
        END
        INSERT INTO @RequestDates (CheckDate)
        VALUES (@FirstMatchingDate);
    END

    -- Kiểm tra xung đột với BookingHolds
    IF EXISTS (
        SELECT 1 
        FROM BookingHolds bh
        CROSS APPLY (
            SELECT CheckDate
            FROM @RequestDates rd
            WHERE (
                (bh.BookingType = 1 AND CAST(bh.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = rd.CheckDate)
                OR (bh.BookingType IN (2, 3) AND bh.DayOfWeek = @EffectiveDayOfWeek
                    AND CAST(bh.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= rd.CheckDate
                    AND (bh.EndAt IS NULL OR CAST(bh.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= rd.CheckDate))
            )
        ) AS ConflictingDates
        WHERE bh.CourtId = @CourtId
          AND bh.TimeSlotId = @TimeSlotId
          AND bh.ExpiresAt > @CurrentDateTime
    )
    BEGIN
        SELECT 0 AS result, N'Không thể giữ lịch, đã có người giữ lịch trước đó.' AS Message;
        RETURN;
    END

    -- Kiểm tra xung đột với BookingDetails
    IF EXISTS (
        SELECT 1 
        FROM BookingDetails bd
        JOIN Bookings b ON bd.BookingId = b.Id
        CROSS APPLY (
            SELECT CheckDate
            FROM @RequestDates rd
            WHERE (
                (b.Type = 1 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = rd.CheckDate)
                OR (b.Type = 2 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= rd.CheckDate
                           AND CAST(bd.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= rd.CheckDate
                           AND bd.DayOfWeek = @EffectiveDayOfWeek)
                OR (b.Type = 3 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= rd.CheckDate
                           AND bd.DayOfWeek = @EffectiveDayOfWeek)
            )
        ) AS ConflictingDates
        WHERE bd.CourtId = @CourtId
          AND bd.TimeSlotId = @TimeSlotId
          AND b.Status NOT IN (0,4, 5, 6)
    )
    BEGIN
        SELECT 0 AS result, N'Không thể giữ lịch, đã có lịch đặt giao thoa.' AS Message;
        RETURN;
    END

    SELECT 1 AS result, N'Có thể giữ lịch' AS Message;
END;
GO

CREATE OR ALTER PROCEDURE GetBookingScheduleForAdmin
    @FacilityId INT,
    @Date DATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy thời gian hiện tại ở UTC+7
    DECLARE @CurrentDateTime DATETIMEOFFSET = SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time';
    DECLARE @CurrentTime TIME = CAST(@CurrentDateTime AS TIME);
    DECLARE @CurrentDate DATE = CAST(@CurrentDateTime AS DATE);
    DECLARE @TodayDayName VARCHAR(20) = DATENAME(WEEKDAY, @Date);

    WITH AllSlots AS (
        SELECT c.Id AS CourtId, 
               c.Name AS CourtName, 
               ts.Id AS TimeSlotId, 
               ts.StartTime, 
               ts.EndTime
        FROM Courts c
        CROSS JOIN TimeSlots ts
        WHERE c.FacilityId = @FacilityId
    ),
    ActiveHolds AS (
        SELECT CourtId, TimeSlotId, HoldId, HeldBy
        FROM (
            SELECT CourtId, TimeSlotId, Id AS HoldId, HeldBy,
                   ROW_NUMBER() OVER(PARTITION BY CourtId, TimeSlotId ORDER BY Id) AS rn
            FROM BookingHolds
            WHERE ExpiresAt > @CurrentDateTime
              AND (
                  (BookingType = 1 AND CAST(BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = @Date)
                  OR (BookingType IN (2, 3) AND DayOfWeek = @TodayDayName 
                      AND CAST(BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= @Date
                      AND (EndAt IS NULL OR CAST(EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= @Date))
              )
        ) AS sub
        WHERE rn = 1
    ),
    ActiveBookings AS (
        SELECT bd.CourtId, 
               bd.TimeSlotId, 
               b.Status, 
               b.Type, 
               bd.BeginAt, 
               bd.EndAt, 
               bd.DayOfWeek,
               b.Id AS BookingId,
               bd.Id AS BookingDetailId
        FROM BookingDetails bd
        JOIN Bookings b ON bd.BookingId = b.Id
        WHERE b.Status NOT IN (5, 6)
          AND (
                (b.Type = 1 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = @Date)
             OR (b.Type = 2 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= @Date 
                          AND CAST(bd.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= @Date 
                          AND bd.DayOfWeek = @TodayDayName)
             OR (b.Type = 3 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= @Date 
                          AND bd.DayOfWeek = @TodayDayName)
          )
    )
    SELECT 
        s.CourtId,
        s.CourtName,
        s.TimeSlotId,
        s.StartTime,
        s.EndTime,
        CASE 
            WHEN h.HoldId IS NOT NULL THEN 2
			WHEN EXISTS (SELECT 1 FROM ActiveBookings ab 
                         WHERE ab.CourtId = s.CourtId 
                           AND ab.TimeSlotId = s.TimeSlotId 
                           AND ab.Status = 0) THEN 8
            WHEN EXISTS (SELECT 1 FROM ActiveBookings ab 
                         WHERE ab.CourtId = s.CourtId 
                           AND ab.TimeSlotId = s.TimeSlotId 
                           AND ab.Status = 1) THEN 3
            WHEN EXISTS (SELECT 1 FROM ActiveBookings ab 
                         WHERE ab.CourtId = s.CourtId 
                           AND ab.TimeSlotId = s.TimeSlotId 
                           AND ab.Status = 2) THEN 4
            WHEN EXISTS (SELECT 1 FROM ActiveBookings ab 
                         WHERE ab.CourtId = s.CourtId 
                           AND ab.TimeSlotId = s.TimeSlotId 
                           AND ab.Status = 3) THEN 6
            WHEN EXISTS (SELECT 1 FROM ActiveBookings ab 
                         WHERE ab.CourtId = s.CourtId 
                           AND ab.TimeSlotId = s.TimeSlotId 
                           AND ab.Status = 4) THEN 5
            WHEN s.StartTime < @CurrentTime AND @Date <= @CurrentDate THEN 0
            WHEN @Date < @CurrentDate THEN 0
            ELSE 1
        END AS [Status],
        h.HoldId,
        h.HeldBy,
        ab.BookingId,
        ab.BookingDetailId
    FROM AllSlots s
    LEFT JOIN ActiveHolds h ON h.CourtId = s.CourtId AND h.TimeSlotId = s.TimeSlotId
    LEFT JOIN ActiveBookings ab ON ab.CourtId = s.CourtId AND ab.TimeSlotId = s.TimeSlotId
    ORDER BY s.CourtId, s.TimeSlotId;
END;
GO

CREATE OR ALTER PROCEDURE GetBookingScheduleForCourtInRange
    @CourtId INT,
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    IF @StartDate > @EndDate
    BEGIN
        SELECT 0 AS result, N'Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc.' AS Message;
        RETURN;
    END

    -- Lấy thời gian hiện tại ở UTC+7
    DECLARE @CurrentDateTime DATETIMEOFFSET = SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time';
    DECLARE @CurrentTime TIME = CAST(@CurrentDateTime AS TIME);
    DECLARE @CurrentDate DATE = CAST(@CurrentDateTime AS DATE);

    WITH DateRange AS (
        SELECT @StartDate AS ScheduleDate
        UNION ALL
        SELECT DATEADD(DAY, 1, ScheduleDate)
        FROM DateRange
        WHERE ScheduleDate < @EndDate
    ),
    AllSlots AS (
        SELECT 
            dr.ScheduleDate,
            DATENAME(WEEKDAY, dr.ScheduleDate) AS DayOfWeek,
            c.Id AS CourtId,
            c.Name AS CourtName,
            ts.Id AS TimeSlotId,
            ts.StartTime,
            ts.EndTime
        FROM DateRange dr
        CROSS JOIN Courts c
        CROSS JOIN TimeSlots ts
        WHERE c.Id = @CourtId
    ),
    ActiveHolds AS (
        SELECT 
            s.ScheduleDate,
            s.CourtId,
            s.TimeSlotId,
            h.HoldId,
            h.HeldBy
        FROM AllSlots s
        OUTER APPLY (
            SELECT TOP 1 
                bh.Id AS HoldId,
                bh.HeldBy
            FROM BookingHolds bh
            WHERE bh.CourtId = s.CourtId
              AND bh.TimeSlotId = s.TimeSlotId
              AND bh.ExpiresAt > @CurrentDateTime
              AND (
                  (bh.BookingType = 1 AND CAST(bh.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = s.ScheduleDate)
                  OR (bh.BookingType IN (2, 3) AND bh.DayOfWeek = s.DayOfWeek 
                      AND CAST(bh.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= s.ScheduleDate
                      AND (bh.EndAt IS NULL OR CAST(bh.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= s.ScheduleDate))
              )
            ORDER BY bh.Id DESC
        ) h
    ),
    ActiveBookings AS (
        SELECT 
            s.ScheduleDate,
            s.CourtId,
            s.TimeSlotId,
            ab.Status,
            ab.Type,
            ab.BeginAt,
            ab.EndAt,
            ab.DayOfWeek,
            ab.BookingId,
            ab.BookingDetailId
        FROM AllSlots s
        OUTER APPLY (
            SELECT TOP 1 
                b.Status,
                b.Type,
                bd.BeginAt,
                bd.EndAt,
                bd.DayOfWeek,
                b.Id AS BookingId,
                bd.Id AS BookingDetailId
            FROM BookingDetails bd
            JOIN Bookings b ON bd.BookingId = b.Id
            WHERE b.Status NOT IN (5, 6)
              AND bd.CourtId = s.CourtId
              AND bd.TimeSlotId = s.TimeSlotId
              AND (
                  (b.Type = 1 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = s.ScheduleDate)
                  OR (b.Type = 2 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= s.ScheduleDate 
                              AND CAST(bd.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= s.ScheduleDate 
                              AND bd.DayOfWeek = s.DayOfWeek)
                  OR (b.Type = 3 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= s.ScheduleDate 
                              AND bd.DayOfWeek = s.DayOfWeek)
              )
            ORDER BY b.Id DESC
        ) ab
    )
    SELECT 
        s.ScheduleDate,
        s.DayOfWeek,
        s.CourtId,
        s.CourtName,
        s.TimeSlotId,
        s.StartTime,
        s.EndTime,
        CASE 
            WHEN h.HoldId IS NOT NULL THEN 2
            WHEN ab.Status = 0 THEN 8
			WHEN ab.Status = 1 THEN 3
            WHEN ab.Status = 2 THEN 4
            WHEN ab.Status = 3 THEN 6
            WHEN ab.Status = 4 THEN 5
            WHEN s.StartTime < @CurrentTime AND s.ScheduleDate = @CurrentDate THEN 0
            WHEN s.ScheduleDate < @CurrentDate THEN 0
            ELSE 1
        END AS [Status],
        h.HoldId,
        h.HeldBy,
        ab.BookingId,
        ab.BookingDetailId
    FROM AllSlots s
    LEFT JOIN ActiveHolds h ON h.CourtId = s.CourtId 
                           AND h.TimeSlotId = s.TimeSlotId 
                           AND h.ScheduleDate = s.ScheduleDate
    LEFT JOIN ActiveBookings ab ON ab.CourtId = s.CourtId 
                                AND ab.TimeSlotId = s.TimeSlotId 
                                AND ab.ScheduleDate = s.ScheduleDate
    ORDER BY s.ScheduleDate, s.TimeSlotId;
END;
GO

CREATE OR ALTER PROCEDURE CheckMultiDaySlotAvailability
    @CourtId INT,
    @StartDate DATETIMEOFFSET,
    @EndDate DATETIMEOFFSET,
    @DaysOfWeek NVARCHAR(100) -- Chuỗi DaysOfWeek cách nhau bởi dấu phẩy (e.g., 'Monday,Tuesday')
AS
BEGIN
    SET NOCOUNT ON;

    -- Lấy thời gian hiện tại ở UTC+7
    DECLARE @CurrentDateTime DATETIMEOFFSET = SYSDATETIMEOFFSET() AT TIME ZONE 'SE Asia Standard Time';
    DECLARE @CurrentTime TIME = CAST(@CurrentDateTime AS TIME);
    DECLARE @CurrentDate DATE = CAST(@CurrentDateTime AS DATE);

    -- Chuyển đổi @StartDate và @EndDate sang UTC+7
    DECLARE @StartDateLocal DATETIMEOFFSET = @StartDate AT TIME ZONE 'SE Asia Standard Time';
    DECLARE @EndDateLocal DATETIMEOFFSET = @EndDate AT TIME ZONE 'SE Asia Standard Time';

    -- Điều chỉnh StartDateLocal để bắt đầu từ hiện tại nếu nó trong quá khứ
    DECLARE @AdjustedStartDateLocal DATETIMEOFFSET = 
        CASE 
            WHEN @StartDateLocal < @CurrentDateTime THEN @CurrentDateTime 
            ELSE @StartDateLocal 
        END;

    -- Tạo bảng tạm để lưu danh sách DaysOfWeek
    DECLARE @DaysOfWeekTable TABLE (DayOfWeek VARCHAR(10));
    INSERT INTO @DaysOfWeekTable
    SELECT TRIM(value) FROM STRING_SPLIT(@DaysOfWeek, ',');

    -- Tìm ngày đầu tiên trong range khớp với DaysOfWeek
    DECLARE @FirstMatchingDate DATE = CAST(@AdjustedStartDateLocal AS DATE);
    WHILE NOT EXISTS (SELECT 1 FROM @DaysOfWeekTable WHERE DayOfWeek = DATENAME(WEEKDAY, @FirstMatchingDate))
          AND @FirstMatchingDate <= CAST(@EndDateLocal AS DATE)
    BEGIN
        SET @FirstMatchingDate = DATEADD(DAY, 1, @FirstMatchingDate);
    END;

    -- Nếu không tìm thấy ngày khớp nào trong range, trả về bảng rỗng
    IF @FirstMatchingDate > CAST(@EndDateLocal AS DATE)
    BEGIN
        SELECT Id FROM TimeSlots WHERE 1 = 0; -- Trả về bảng rỗng
        RETURN;
    END;

    -- Tạo danh sách ngày trong khoảng thời gian theo DaysOfWeek
    WITH DateRange AS (
        SELECT @FirstMatchingDate AS CheckDate
        UNION ALL
        SELECT DATEADD(DAY, 1, CheckDate)
        FROM DateRange
        WHERE CheckDate < CAST(@EndDateLocal AS DATE)
          AND EXISTS (SELECT 1 FROM @DaysOfWeekTable WHERE DayOfWeek = DATENAME(WEEKDAY, DATEADD(DAY, 1, CheckDate)))
    ),
    -- Lấy tất cả TimeSlotId từ TimeSlots cùng với StartTime
    AllTimeSlots AS (
        SELECT Id AS TimeSlotId, StartTime
        FROM TimeSlots
        WHERE IsApplied = 1 AND IsDeleted = 0
    ),
    -- Đếm số ngày cần kiểm tra
    TotalDays AS (
        SELECT COUNT(*) AS TotalDaysCount
        FROM DateRange
    ),
    -- Kiểm tra xung đột với BookingHolds
    ConflictingHolds AS (
        SELECT 
            ats.TimeSlotId,
            dr.CheckDate,
            CASE 
                WHEN EXISTS (
                    SELECT 1
                    FROM BookingHolds bh
                    WHERE bh.CourtId = @CourtId
                      AND bh.TimeSlotId = ats.TimeSlotId
                      AND bh.ExpiresAt > @CurrentDateTime
                      AND (
                          (bh.BookingType = 1 AND CAST(bh.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = dr.CheckDate)
                          OR (bh.BookingType IN (2, 3) AND bh.DayOfWeek = DATENAME(WEEKDAY, dr.CheckDate)
                              AND CAST(bh.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= dr.CheckDate
                              AND (bh.EndAt IS NULL OR CAST(bh.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= dr.CheckDate))
                      )
                ) THEN 1
                ELSE 0
            END AS HasConflict
        FROM AllTimeSlots ats
        CROSS JOIN DateRange dr
    ),
    -- Kiểm tra xung đột với BookingDetails
    ConflictingBookings AS (
        SELECT 
            ats.TimeSlotId,
            dr.CheckDate,
            CASE 
                WHEN EXISTS (
                    SELECT 1
                    FROM BookingDetails bd
                    JOIN Bookings b ON bd.BookingId = b.Id
                    WHERE bd.CourtId = @CourtId
                      AND bd.TimeSlotId = ats.TimeSlotId
                      AND b.Status NOT IN (4, 5, 6) -- Loại bỏ canceled, paused, completed
                      AND (
                          (b.Type = 1 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) = dr.CheckDate)
                          OR (b.Type = 2 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= dr.CheckDate
                                     AND CAST(bd.EndAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) >= dr.CheckDate
                                     AND bd.DayOfWeek = DATENAME(WEEKDAY, dr.CheckDate))
                          OR (b.Type = 3 AND CAST(bd.BeginAt AT TIME ZONE 'UTC' AT TIME ZONE 'SE Asia Standard Time' AS DATE) <= dr.CheckDate
                                     AND bd.DayOfWeek = DATENAME(WEEKDAY, dr.CheckDate))
                      )
                ) THEN 1
                ELSE 0
            END AS HasConflict
        FROM AllTimeSlots ats
        CROSS JOIN DateRange dr
    ),
    -- Tổng hợp số ngày có xung đột cho mỗi TimeSlot, thêm kiểm tra timeout
    ConflictSummary AS (
        SELECT 
            ats.TimeSlotId,
            SUM(ch.HasConflict + cb.HasConflict) AS ConflictCount,
            td.TotalDaysCount,
            -- Kiểm tra timeout: nếu CheckDate là ngày hiện tại và StartTime < CurrentTime thì coi như có xung đột
            MAX(CASE 
                WHEN dr.CheckDate = @CurrentDate AND ats.StartTime < @CurrentTime THEN 1
                ELSE 0
            END) AS IsTimeout
        FROM AllTimeSlots ats
        CROSS JOIN TotalDays td
        LEFT JOIN ConflictingHolds ch ON ch.TimeSlotId = ats.TimeSlotId
        LEFT JOIN ConflictingBookings cb ON cb.TimeSlotId = ats.TimeSlotId AND cb.CheckDate = ch.CheckDate
        LEFT JOIN DateRange dr ON ch.CheckDate = dr.CheckDate OR cb.CheckDate = dr.CheckDate
        GROUP BY ats.TimeSlotId, td.TotalDaysCount
    )
    -- Trả về các TimeSlotId không có xung đột và không timeout
    SELECT TimeSlotId
    FROM ConflictSummary
    WHERE ConflictCount = 0 AND IsTimeout = 0;
END;
GO