using GuardianStock.Application.Common.Interfaces;
using System.Security.Claims;

namespace GuardianStock.API.Contracts
{
    public sealed class CurrentUser : ICurrentUser
    {

        private readonly HttpContext? _httpContext;

        public CurrentUser(IHttpContextAccessor? httpContextAccessor)
        {

            _httpContext = httpContextAccessor?.HttpContext;
        }

        public Guid? UserId
        {
            get
            {

                if (_httpContext == null)
                    return null;

                ClaimsPrincipal? user = _httpContext.User;

                bool IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

                if (!IsAuthenticated)
                    return null;

                var claim = user!.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(claim, out var userId) ? userId : null;

            }

        }

        public string UserEmail
        {
            get
            {

                if (_httpContext == null)
                    return "System";

                ClaimsPrincipal? user = _httpContext.User;

                bool IsAuthenticated = user?.Identity?.IsAuthenticated ?? false;

                if (!IsAuthenticated)
                    return "Anonymous";

                return user!.FindFirst(ClaimTypes.Email)?.Value ?? "Unkown";

            }

        }


    }
}
