using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(new { id = id, name = "banana" });
        }
        [HttpPost]
        public IActionResult Post([FromBody] string value)
        {
            return Ok(new {id = 1, name = value });
        }
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { id = 1, name = "banana" });
        }
    }
}
