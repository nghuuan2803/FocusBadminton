using Application.Features.Auth.Factories;
using Microsoft.AspNetCore.Mvc;
using Web.Request.Auth;

namespace Web.Endpoints
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginStrategyFactory _loginStrategyFactory;
        public AuthController(ILoginStrategyFactory loginStrategyFactory)
        {
            _loginStrategyFactory = loginStrategyFactory;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthLoginRequest request)
        {
            // 📌 Chọn strategy dựa vào request.LoginType ("google", "password", "facebook", ...)
            var strategy = _loginStrategyFactory.GetStrategy(request.LoginType);
            var result = await strategy.LoginAsync(request.Credential);

            if (!result.Succeeded)
            {
                return BadRequest(new { error = result.Errors });
            }
            return Ok(result.Data);
        }
    }
}
