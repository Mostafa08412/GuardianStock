using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMS.API.Middleware
{

    public static class ExceptionHandlerMiddlewareDI
    {

        public static void UseGloabalExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
    public class ExceptionHandlerMiddleware
    {

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;



        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }

            catch (DbUpdateConcurrencyException)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "A concurrency conflict occurred. The record may have been modified by another user.",
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8"


                });
            }

            catch (DbUpdateException ex)
            {
                context.Response.StatusCode = StatusCodes.Status409Conflict;

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status409Conflict,
                    Title = "A database update error occured",
                    Detail = ex.InnerException?.Message ?? ex.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                    Extensions =
                    {
                        ["instance"] = context.Request.Path.Value,
                        ["traceId"] = context.TraceIdentifier
                    }

                });
            }

            catch (ArgumentNullException ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;


                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An error occured due to missing section.",
                    Detail = ex.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Extensions =
                    {
                        ["instance"] = context.Request.Path.Value,
                        ["traceId"] = context.TraceIdentifier
                    }
                });
            }

            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;


                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred.",
                    Detail = ex.Message,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Extensions =
                    {
                        ["instance"] = context.Request.Path.Value,
                        ["traceId"] = context.TraceIdentifier
                    }
                });
            }

        }
    }
}
