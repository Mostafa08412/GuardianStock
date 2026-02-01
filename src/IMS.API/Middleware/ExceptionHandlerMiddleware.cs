using IMS.API.Infrastructure;
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
            var helper = new ApiResponseHelper(context);

            try
            {
                await _next.Invoke(context);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception of type {ExceptioType} occurred: {Message}", ex.GetType().Name, ex.Message);
                var value = HandleException(ex);
                await WriteResponseAsync(value.Item1, value.Item2, value.Item3, context);
            }

        }


        public (string, string, ApplicationStatusCodes) HandleException(Exception ex)
        {
            return ex switch
            {
                DbUpdateConcurrencyException =>
                (
                "The record has been modified by another user. Please refresh and try again.",
                "DATABASE.CONCURRENCY",
                ApplicationStatusCodes.Conflict),

                DbUpdateException =>
                (
                "A database integrity error occurred.",
                "DATABASE.UPDATE_ERROR",
                ApplicationStatusCodes.Conflict),

                ArgumentNullException =>
                (
                "A required system configuration is missing.",
                "SERVER.MISSING_CONFIG",
                ApplicationStatusCodes.InternalServerError),

                _ =>
                (
                "An internal server error occurred.",
                "SERVER.UNEXPECTED_ERROR",
                ApplicationStatusCodes.InternalServerError),

            };
        }

        public async Task WriteResponseAsync(string message, string errorCode, ApplicationStatusCodes statusCode, HttpContext context)
        {
            var Helper = new ApiResponseHelper(context);

            var response = Helper.BasicErrorApiResponse(message, errorCode);

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
