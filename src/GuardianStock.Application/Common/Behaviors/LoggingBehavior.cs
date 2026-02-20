using GuardianStock.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace GuardianStock.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly ICurrentUser _currentUser;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUser currentUser)
        {
            _logger = logger;
            _currentUser = currentUser;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userEmail = _currentUser.UserEmail ?? "Anonymous";
            var userId = _currentUser.UserId?.ToString() ?? "N/A";

            _logger.LogInformation("⚙️ Starting Request: {RequestName} | User: email={UserEmail} Id=({UserId}) Parameters: {request}",
                requestName, userEmail, userId, request);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                var response = await next();

                // 2. Success Log (Post-Execution)
                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds > 200)
                {
                    _logger.LogWarning("⚠️ Request took longer than expected: {RequestName} | User: {UserEmail} | Duration: {ElapsedMilliseconds}ms",
                        requestName, userEmail, stopwatch.ElapsedMilliseconds);
                }
                else
                {

                    _logger.LogInformation("✅ Completed Request: {RequestName} | User: {UserEmail} | Duration: {ElapsedMilliseconds}ms",
                        requestName, userEmail, stopwatch.ElapsedMilliseconds);
                }


                return response;
            }
            catch (Exception ex)
            {

                stopwatch.Stop();

                if (stopwatch.ElapsedMilliseconds > 200)
                {
                    _logger.LogWarning(ex, "Request Failed and took longer than expected: {RequestName} | User: {UserEmail} | Duration: {ElapsedMilliseconds}ms | Error: {ErrorMessage}",
                        requestName, userEmail, stopwatch.ElapsedMilliseconds, ex.Message);
                }
                else
                {
                    _logger.LogError(ex, "Request Failed: {RequestName} | User: {UserEmail} | Duration: {ElapsedMilliseconds}ms | Error: {ErrorMessage}",
                        requestName, userEmail, stopwatch.ElapsedMilliseconds, ex.Message);

                }


                throw;
            }
        }
    }
}
