using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Interfaces;
using static WebhookTester.API.Utils.StatusCodeUtils;

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
        public async Task<IActionResult> Post()
        {
            var result = await service.CreateToken();
            return Ok(result.Data.Id);
        }

        /// <summary>
        /// Refresh an existing token
        /// </summary>
        /// <returns></returns>
        [HttpPatch()]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Patch(string token)
        {
            //TODO: consider receiving token throught header
            var result = await service.RefreshToken(token);
            if (!result.Success)
            {
                return MapErrorToResult(result.Error);
            }
            return Ok();
        }
    }
}
