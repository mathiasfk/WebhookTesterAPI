using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebhookTester.Core.Common;
using WebhookTester.Core.Interfaces;

namespace WebhookTester.API.ActionFilters
{
    /// <summary>
    /// Validates the token in the Authorization header.
    /// </summary>
    /// <param name="tokenService"></param>
    public class ValidateTokenFilter(ITokenService tokenService) : IAsyncActionFilter
    {
        /// <summary>
        /// Called before each endpoint is executed. Validates the token and sets the token ID in the HttpContext.Items dictionary.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var tokenResult = await tokenService.ValidateToken(context.HttpContext.Request.Headers.Authorization.ToString());
            if (!tokenResult.Success)
            {
                context.Result = new ObjectResult(tokenResult.Error?.Message)
                {
                    StatusCode = tokenResult.Error?.Code switch
                    {
                        ErrorCode.BadRequest => StatusCodes.Status400BadRequest,
                        ErrorCode.Unauthorized => StatusCodes.Status401Unauthorized,
                        ErrorCode.NotFound => StatusCodes.Status404NotFound,
                        _ => StatusCodes.Status500InternalServerError,
                    }
                };
                return;
            }

            context.HttpContext.Items["Token"] = tokenResult.Data.Id;
            await next();
        }
    }

}
