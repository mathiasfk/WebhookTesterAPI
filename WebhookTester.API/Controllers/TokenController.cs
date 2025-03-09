using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.API.Controllers
{
    /// <summary>
    /// Handles token generation.
    /// </summary>
    /// <param name="service"></param>
    [ApiController]
    [Route("[controller]")]
    public class TokenController(ITokenService service) : ControllerBase
    {
        /// <summary>
        /// Create a new authentication token
        /// </summary>
        /// <returns>The created token</returns>
        [HttpPost()]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        public IActionResult Post()
        {
            var token = service.CreateToken();
            return Ok(token.Data);
        }
    }
}
