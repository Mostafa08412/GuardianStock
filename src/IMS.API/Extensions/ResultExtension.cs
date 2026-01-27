using IMS.Domain.Core.Primitives.Result;
using Microsoft.AspNetCore.Mvc;

namespace IMS.API.Extensions
{
    public static class ResultExtension
    {




        public static IActionResult Map(this Result result, Func<Result, IActionResult> onSuccess, Func<Result, IActionResult> onFailure)
        {
            if (result.IsSuccess)
                return onSuccess(result);

            else

                return onFailure(result);
        }

        public static IActionResult Map<T>(this Result<T> result, Func<Result<T>, IActionResult> onSuccess, Func<Result<T>, IActionResult> onFailure)
        {
            if (result.IsSuccess)
                return onSuccess(result);

            else

                return onFailure(result);
        }
    }
}
