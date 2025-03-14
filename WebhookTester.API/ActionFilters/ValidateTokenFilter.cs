using Microsoft.AspNetCore.Mvc.Filters;
using WebhookTester.Core.Interfaces;
using static WebhookTester.API.Utils.StatusCodeUtils;

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
                context.Result = MapErrorToResult(tokenResult.Error);
                return;
            }

            context.HttpContext.Items["Token"] = tokenResult.Data.Id;
            await next();
        }
    }

}
