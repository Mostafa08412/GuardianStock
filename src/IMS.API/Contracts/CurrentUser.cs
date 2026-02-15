using IMS.Application.Common.Interfaces;
using System.Security.Claims;

namespace IMS.API.Contracts
{
    public sealed class CurrentUser : ICurrentUser
    {

        private readonly HttpContext? _httpContext;

        public CurrentUser(IHttpContextAccessor? httpContextAccessor)
        {

            _httpContext = httpContextAccessor?.HttpContext;
        }

        public string UserId
        {
            get
            {

                if (_httpContext == null)
                    return "System";

                ClaimsPrincipal? user = _httpContext.User;

                bool IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

                if (!IsAuthenticated)
                    return "Anonymous";

                return user!.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unkown";

            }

        }


    }
}
