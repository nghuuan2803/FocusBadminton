using Application.Features.TimeSlots.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSlotsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TimeSlotsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellation)
        {
            var result = await _mediator.Send(new GetTimeSlotsQuery(), cancellation);
            return Ok(result);
        }
    }
}
