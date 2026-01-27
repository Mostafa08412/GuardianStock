using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Infrastructure
{


    public class BaseController : ControllerBase
    {

        internal ISender _sender;

        public BaseController(ISender sender)
        {
            _sender = sender;
        }

        protected IActionResult MapResultFailureToActionResult(Result result)
        {

            var primaryError = result.Errors.First();

            // Validation Error → ValidationProblemDetails
            if (primaryError.ErrorType == ErrorType.Validation)
            {
                var errorsDict = result.Errors
                    .GroupBy(e => e.Code)
                    .ToDictionary(
                        g => g.Key.ToString(),
                        g => g.Select(e => e.Description).ToArray()
                    );

                var validationProblem = new ValidationProblemDetails(errorsDict)
                {
                    Title = "Validation Failed",
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    Extensions =
                    {
                        ["instance"] = HttpContext.Request.Path.Value,
                        ["traceId"] = HttpContext.TraceIdentifier
                    }

                };

                return BadRequest(validationProblem);
            }


            // Otherwise → ProblemDetails
            var problemDetails = new ProblemDetails
            {
                Title = primaryError.Description,
                Detail = string.Join("; ", result.Errors.Select(e => e.Description)),
                Extensions =
                    {
                        ["instance"] = HttpContext.Request.Path.Value,
                        ["traceId"] = HttpContext.TraceIdentifier
                    }

            };

            switch (primaryError.ErrorType)
            {


                case ErrorType.NotFound:
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    problemDetails.Type =
                        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4";
                    return NotFound(problemDetails);

                case ErrorType.Conflict:
                    problemDetails.Status = StatusCodes.Status409Conflict;
                    problemDetails.Type =
                        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8";
                    return Conflict(problemDetails);

                case ErrorType.AccessDenied:
                    problemDetails.Status = StatusCodes.Status403Forbidden;
                    problemDetails.Type =
                        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3";
                    return StatusCode(StatusCodes.Status403Forbidden, problemDetails);

                case ErrorType.ConditionNotMet:
                    problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                    problemDetails.Type =
                        "https://datatracker.ietf.org/doc/html/rfc4918#section-11.2";
                    return UnprocessableEntity(problemDetails);

                case ErrorType.Failure:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Type =
                        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                    return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);

                default:
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    problemDetails.Type =
                        "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1";
                    return StatusCode(StatusCodes.Status500InternalServerError, problemDetails);
            }
        }
    }

}

