using Application.Features.HoldSlots;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SlotController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("hold")]
        public async Task<IActionResult> HoldSlot([FromBody] HoldSlotCommand command)
        {
            //return hold id
            var result = await _mediator.Send(command);
            if (result != 0)
            {

                return Ok(result);
            }
            return BadRequest("Slot không khả dụng để giữ.");
        }

        [HttpPost("release")]
        public async Task<IActionResult> ReleaseSlot([FromBody] ReleaseSlotCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
