using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController(ITokenService service) : ControllerBase
    {
        private readonly ITokenService _service = service;

        [HttpPost()]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public IActionResult Post()
        {
            var token = _service.CreateToken();
            return Ok(token);
        }
    }
}
