using Application.Features.Slots;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;
using Shared.Schedules;
using System.Threading;

namespace Infrastructure.Data.Queries
{
    public class CheckMultiDaySlotAvailability : ICheckMultiDaySlotAvailabilityQuery
    {
        private readonly AppDbContext _dbContext;

        public CheckMultiDaySlotAvailability(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<int>> Execute(CheckMultiDaySlotAvailabilityQuery request, CancellationToken cancellation = default)
        {
            var daysOfWeekParam = string.Join(",", request.DaysOfWeek);
            var availableTimeSlotIds = await _dbContext.Database
                .SqlQuery<int>(
                    $"EXEC CheckMultiDaySlotAvailability @CourtId={request.CourtId}, @StartDate={request.StartDate}, @EndDate={request.EndDate}, @DaysOfWeek={daysOfWeekParam}")
                .ToListAsync(cancellation);

            return availableTimeSlotIds;
        }
    }
}
