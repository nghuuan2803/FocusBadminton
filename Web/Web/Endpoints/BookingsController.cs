using Application.Features.Bookings.Commands;
using Application.Features.Bookings.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IMediator mediator) : ControllerBase
    {
        #region commands
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingCommand command)
        {
            command.HttpContext = HttpContext;
            var result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings()
        {
            var query = new GetBookingsQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }
        [HttpPost("approve")]
        public async Task<IActionResult> ApproveBooking(ApproveBookingCommand command)
        {

            var result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectBooking(RejectBookingCommand command)
        {

            var result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("pause")]
        public async Task<IActionResult> PauseBooking(PauseBookingCommand command)
        {
            var result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }
        [HttpPost("resume")]
        public async Task<IActionResult> ResumeBooking(ResumeBookingCommand command)
        {
            var result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> CancelBooking(CancelBookingCommand command)
        {
            var result = await mediator.Send(command);
            if (result.Succeeded)
            {
                return Ok(true);
            }
            return BadRequest(result.Errors);
        }
        #endregion

        #region queries
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var query = new GetBookingQuery(id);
            var result = await mediator.Send(query);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }

        [HttpGet("history/{memberId}")]
        public async Task<IActionResult> GetHistory(int memberId)
        {
            var query = new GetBookingHistoryQuery { MemberId = memberId};
            var result = await mediator.Send(query);
            if (result.Succeeded)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.Errors);
        }
        #endregion
    }
}