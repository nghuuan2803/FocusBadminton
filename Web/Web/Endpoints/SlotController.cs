using Application.Features.Slots;
using MediatR;
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
            var result = await _mediator.Send(command);
            if (result != null)
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
                return Ok(true);
            }
            return BadRequest("Slot không thể nhả");
        }

        [HttpPost("check-fixed-booking-hold")]
        public async Task<IActionResult> CheckMultiDayAvailability([FromBody] FixedBookingHoldCommand request)
        {
            var result = await _mediator.Send(request);
            if (result.Succeeded)
            {
                return Ok(new { data = result.Data, succeeded = true, errors = new string[0] });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("release-multi")]
        public async Task<IActionResult> ReleaseMultipleSlots([FromBody] ReleaseMultipleSlotsCommand command)
        {
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(true);
            }
            return BadRequest("Không thể nhả một hoặc nhiều slot");
        }
    }
}
