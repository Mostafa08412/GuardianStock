using IMS.Application.Common.Interfaces;
using System.Security.Claims;

namespace IMS.API.Contracts
{
    public sealed class CurrentUser : ICurrentUser
    {

        private readonly HttpContext httpContext;

        public CurrentUser(IHttpContextAccessor? httpContextAccessor)
        {

            this.httpContext = httpContextAccessor!.HttpContext!;
        }

        public string UserId => httpContext!.User.Claims.FirstOrDefault(X => X.Type == ClaimTypes.NameIdentifier)!.Value ?? string.Empty;
    }
}
