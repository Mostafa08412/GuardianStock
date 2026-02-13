using IMS.API.Infrastructure;
using IMS.Domain.Core.Errors;
namespace IMS.API.Middleware
{
    public class HandleAuthenticationErrorMiddleware
    {
        private RequestDelegate next;

        public HandleAuthenticationErrorMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var helper = new ApiResponseHelper(context);


            await next.Invoke(context);

            if (context.Response.Headers.ContainsKey("Auth-Fail-Type"))
            {
                string headerValue = context.Response.Headers["Auth-Fail-Type"];

                if (!string.IsNullOrEmpty(headerValue))
                {
                    if (headerValue == Errors.Identity.InvalidToken.Code)
                    {
                        var error = Errors.Identity.InvalidToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }
                    if (headerValue == IMS.Domain.Core.Errors.Errors.Identity.MissingToken.Code)
                    {
                        var error = Errors.Identity.MissingToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }

                    if (headerValue == IMS.Domain.Core.Errors.Errors.Identity.ExpiredToken.Code)
                    {
                        var error = Errors.Identity.ExpiredToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }

                    if (headerValue == IMS.Domain.Core.Errors.Errors.Identity.ForbiddenAccess.Code)
                    {
                        var error = Errors.Identity.ForbiddenAccess;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Forbidden;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }
                }
            }

        }
    }
}
