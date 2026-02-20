using Swashbuckle.AspNetCore.Filters;

namespace GuardianStock.API.Contracts.Examples
{
    public class ApiResponseExample : IExamplesProvider<ApiResponse>
    {
        public ApiResponse GetExamples()
        {
            return new ApiResponse
            (
                 true,
                "<human-readable message>",
                 "<ERROR_CODE>",
                 new Dictionary<string, string>()
                 {
                     ["email"] = "<email error>",
                     ["password"] = "<password error>"
                 },
                new Dictionary<string, string>()
                {
                    ["page"] = "<page>",
                    ["pageSize"] = "<pageSize>"
                },
                "<request path>",
                "<trace id>"
            );
        }
    }

}