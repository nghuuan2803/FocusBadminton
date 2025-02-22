using Application.Features.Courts.Commands;
using Application.Features.Courts.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourtsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourts()
        {
            var courts = await _mediator.Send(new GetCourtsQuery());
            return Ok(courts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourt(int id)
        {
            var result = await _mediator.Send(new GetCourtQuery() { Id = id });
            if(!result.Succeeded)
            {
                return NotFound(result.Errors);
            }
            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourt([FromBody] CreateCourtCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return CreatedAtAction(nameof(CreateCourt), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourt(int id, [FromBody] UpdateCourtCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(result.Data);
        }
    }
}
