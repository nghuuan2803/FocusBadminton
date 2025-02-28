using Application.Interfaces.DapperQueries;
using Dapper;
using Shared.Enums;
using Shared.Schedules;
using System.Data;

namespace Infrastructure.Data.DapperQueries
{
    public class CourtScheduleQueries : IScheduleQueries
    {
        private readonly DapperSqlConnection _sqlConnection;

        public CourtScheduleQueries(DapperSqlConnection sqlConnection)
        {
            _sqlConnection = sqlConnection;
        }

        public async Task<bool> CheckAvailable(int courtId, int timeSlotId, BookingType bookingType, 
            DateTimeOffset beginAt, DateTimeOffset? endAt, string dayOfWeek)
        {
            using var connection = _sqlConnection.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("CourtId", courtId);
            parameters.Add("TimeSlotId", timeSlotId);
            parameters.Add("BookingType", (int)bookingType);
            parameters.Add("BeginAt", beginAt, DbType.DateTimeOffset);
            parameters.Add("EndAt", endAt, DbType.DateTimeOffset);
            parameters.Add("DayOfWeek", dayOfWeek, DbType.String);

            return await connection.QueryFirstOrDefaultAsync<bool>(
                "CheckBookingHold",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<ScheduleDTO>> GetAllCourtSchedulesAsync(DateTime date, int facilityId )
        {
            using var connection = _sqlConnection.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("FacilityId", facilityId);
            parameters.Add("Date", date.Date, DbType.Date);

            return await connection.QueryAsync<ScheduleDTO>(
                "GetBookingScheduleForAdmin",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<ScheduleDTO>> GetCourtSchedulesAsync(DateTime date, int courtId)
        {
            using var connection = _sqlConnection.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("Date", date.Date, DbType.Date);
            parameters.Add("CourtId", courtId);

            var data = await connection.QueryAsync<ScheduleDTO>(
                "GetBookingScheduleForCourt",
                parameters,
                commandType: CommandType.StoredProcedure
            );
            return data;
        }
    }
}
