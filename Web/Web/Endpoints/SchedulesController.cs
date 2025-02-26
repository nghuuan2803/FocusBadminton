using Application.Features.Schedules;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

    }
}
