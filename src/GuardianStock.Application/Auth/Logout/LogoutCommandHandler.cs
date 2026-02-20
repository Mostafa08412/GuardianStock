using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IIdentityService _identityService;

        public LogoutCommandHandler(ICurrentUser currentUser, IIdentityService identityService)
        {
            _currentUser = currentUser;
            _identityService = identityService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.RevokeActiveRefreshTokenAsync(_currentUser.UserId!.Value);
        }
    }
}
