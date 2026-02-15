using IMS.API.Infrastructure;
using IMS.Application.Common.Errors;
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
                    if (headerValue == ApplicationErrors.IdentityErrors.InvalidToken.Code)
                    {
                        var error = ApplicationErrors.IdentityErrors.InvalidToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }
                    if (headerValue == ApplicationErrors.IdentityErrors.MissingToken.Code)
                    {
                        var error = ApplicationErrors.IdentityErrors.MissingToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }

                    if (headerValue == ApplicationErrors.IdentityErrors.ExpiredToken.Code)
                    {
                        var error = ApplicationErrors.IdentityErrors.ExpiredToken;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Unauthorized;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }

                    if (headerValue == ApplicationErrors.IdentityErrors.ForbiddenAccess.Code)
                    {
                        var error = ApplicationErrors.IdentityErrors.ForbiddenAccess;
                        context.Response.StatusCode = (int)ApplicationStatusCodes.Forbidden;
                        await context.Response.WriteAsJsonAsync(helper.BasicErrorApiResponse(error.Description, error.Code));
                    }
                }
            }

        }
    }
}
