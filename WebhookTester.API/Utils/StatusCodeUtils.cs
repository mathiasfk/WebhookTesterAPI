using Microsoft.AspNetCore.Mvc;
using WebhookTester.Core.Common;

namespace WebhookTester.API.Utils
{
    /// <summary>
    /// Utility class for handling status codes
    /// </summary>
    public static class StatusCodeUtils
    {
        /// <summary>
        /// Maps an error to an IActionResult
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static IActionResult MapErrorToResult(Error? error)
        {
            return new ObjectResult(error?.Message)
            {
                StatusCode = error?.Code switch
                {
                    ErrorCode.BadRequest => StatusCodes.Status400BadRequest,
                    ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
                    ErrorCode.NotFound => StatusCodes.Status404NotFound,
                    ErrorCode.InternalError => StatusCodes.Status500InternalServerError,
                    _ => StatusCodes.Status500InternalServerError,
                }
            };
        }
    }
}
