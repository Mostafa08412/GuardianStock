using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Core.Primitives;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.API.Contracts;
using MediatR;

namespace GuardianStock.API.Infrastructure
{
    public class ApiResponseHelper
    {
        private readonly HttpContext context;

        public ApiResponseHelper(HttpContext context)
        {
            this.context = context;
        }

        public int CalculateStatusCodeFromResult(Result result, ApplicationStatusCodes onSuccess)
        {
            if (result.IsSuccess) return (int)onSuccess;

            return result.Error!.ErrorType switch
            {
                ErrorType.Validation or ErrorType.Failure => (int)ApplicationStatusCodes.BadRequest,
                ErrorType.IdentityError => (int)ApplicationStatusCodes.Unauthorized,
                ErrorType.NotFound => (int)ApplicationStatusCodes.NotFound,
                ErrorType.Conflict => (int)ApplicationStatusCodes.Conflict,
                ErrorType.AccessDenied => (int)ApplicationStatusCodes.Forbidden,
                ErrorType.ConditionNotMet => (int)ApplicationStatusCodes.UnprocessableEntity,
                _ => (int)ApplicationStatusCodes.InternalServerError
            };
        }

        private static (bool isSuccess, string message, string errorCode, Dictionary<string, string> validationErrors)
            GetResponseGeneralData(Result result)
        {
            bool isSuccess = result.IsSuccess;
            string message = result.Message ?? string.Empty;
            string errorCode = string.Empty;
            Dictionary<string, string> validationErrors = new();

            // Extract first non-validation error code
            var nonValidationError = result.Errors.FirstOrDefault(x => x.ErrorType != ErrorType.Validation);
            if (nonValidationError is not null)
            {

                errorCode = nonValidationError.Code;
                message = nonValidationError.Description;
            }

            // Map validation errors
            var validationTypeErrors = result.Errors.Where(x => x.ErrorType == ErrorType.Validation);
            if (validationTypeErrors.Any())
            {
                errorCode = "VALIDATION_ERROR";
                message = "One or more validation errors have occurred.";
            }
            foreach (var error in validationTypeErrors)
            {
                var fieldName = error.Code.Contains('-') ? error.Code.Split('-')[1] : "General";
                var fieldErrorCode = error.Code.Contains('-') ? error.Code.Split('-')[0] : error.Code;

                validationErrors.TryAdd(fieldName.ToLowerInvariant(), error.Description);
            }

            return (isSuccess, message, errorCode, validationErrors);
        }

        public ApiResponse ResultToResponse(Result result)
        {
            var (isSuccess, message, errorCode, validationErrors) = GetResponseGeneralData(result);

            var response = new ApiResponse(
                isSuccess,
                message,
                errorCode,
                validationErrors,
                new Dictionary<string, string>(),
                context.Request.Path.Value ?? "Unknown",
                context.TraceIdentifier);

            return response;
        }

        public ApiResponse<T> ResultToResponse<T>(Result<T> result)
        {
            var (isSuccess, message, errorCode, validationErrors) = GetResponseGeneralData(result);
            Dictionary<string, string> meta = new();
            object? data = result.Value;

            // Check for Pagination
            if (result.Value is IPaginationMetadata paginated)
            {
                meta.Add("pageNumber", paginated.PageNumber.ToString());
                meta.Add("pageSize", paginated.PageSize.ToString());
                meta.Add("totalCount", paginated.TotalCount.ToString());
                meta.Add("totalPages", paginated.TotalPages.ToString());
                meta.Add("hasNext", paginated.HasNext.ToString());
                meta.Add("hasPrevious", paginated.HasPrevious.ToString());

                // Extract "Items" from the PaginatedList<T>
                // We use dynamic to access 'Items' since we know it exists on PaginatedList<T>
                data = ((dynamic)result.Value!) ?? Unit.Value;

            }

            var response = new ApiResponse<T>(
                isSuccess,
                message,
                errorCode,
                validationErrors,
                meta,
                context.Request.Path.Value ?? "Unknown",
                context.TraceIdentifier,
                data: (T)data!)
            {
            };

            return response;
        }


        public ApiResponse BasicErrorApiResponse(string message, string errorCode)
        {
            return new ApiResponse(false, message, errorCode, new Dictionary<string, string>(), new Dictionary<string, string>(), context.Request.Path.Value ?? "Unkown", context.TraceIdentifier);
        }

    }
}
