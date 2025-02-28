using Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Web.Request.Auth;

namespace Web.Endpoints
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> LoginByGoogle([FromBody] LoginByGoogleRequest request)
        {
            var command = new LoginByGoogleCommand(request.Code);
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
            {
                return BadRequest(new { error = result.Errors });
            }
            return Ok(result.Data);
        }
    }
}
