using Application.Features.Schedules;
using Application.Features.Slots;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Schedules;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SchedulesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("facility")]
        public async Task<IActionResult> GetFacilitySchedule([FromQuery] GetCourtSchedulesQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("court")]
        public async Task<IActionResult> GetCourtSchedule([FromQuery] GetCourtScheduleQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("court-range")]
        public async Task<IActionResult> GetCourtSchedule([FromQuery] GetCourtScheduleInRangeQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("check-multi-day-available")]
        public async Task<IActionResult> CheckMultiDayAvailable([FromBody] CheckMultiDayRequest request)
        {
            var availableTimeSlotIds = await _mediator.Send(new CheckMultiDaySlotAvailabilityQuery
            {
                CourtId = request.CourtId,
                StartDate = request.StartDate,
                EndDate = request.EndDate ?? request.StartDate.AddDays(30),
                DaysOfWeek = request.DaysOfWeek
            });

            return Ok(availableTimeSlotIds);
        }

        public class CheckMultiDayRequest
        {
            public int CourtId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public List<string> DaysOfWeek { get; set; } = [];
        }
    }
}
