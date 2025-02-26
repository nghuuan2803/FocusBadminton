using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(AuthService service) : ControllerBase
    {
        [HttpPost("login-google")]
        public async Task<IActionResult> LoginGoogle([FromBody] GoogleAuthRequest request)
        {
            var result = await service.LoginByGoogleAsync(request.IdToken);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok(new { accessToken = result.Data.accessToken, refreshToken = result.Data.refreshToken });
        }
    }
    public class GoogleAuthRequest
    {
        public string IdToken { get; set; }
    }
}
