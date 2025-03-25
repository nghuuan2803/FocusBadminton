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
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingCommand command)
        {
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
    }
}