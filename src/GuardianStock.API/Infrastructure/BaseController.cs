using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GuardianStock.API.Infrastructure
{


    public class BaseController : ControllerBase
    {

        internal ISender _sender;

        public BaseController(ISender sender)
        {
            _sender = sender;
        }

        protected IActionResult HandleResult<T>(Result<T> result, ApplicationStatusCodes onSuccess)
        {
            var helper = new ApiResponseHelper(HttpContext);
            var response = helper.ResultToResponse(result);
            var statusCode = helper.CalculateStatusCodeFromResult(result, onSuccess);
            return StatusCode(statusCode, response);

        }
        protected IActionResult HandleResult(Result result, ApplicationStatusCodes onSuccess)
        {
            var helper = new ApiResponseHelper(HttpContext);
            var response = helper.ResultToResponse(result);
            var statusCode = helper.CalculateStatusCodeFromResult(result, onSuccess);
            return StatusCode(statusCode, response);

        }

    }


}

