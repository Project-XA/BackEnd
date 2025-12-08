using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project_X.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        [HttpPost("Create-Session")]
        public async Task<IActionResult> CreateSession()
        {
            // I will back here
            return Ok();
        }
    }
}
