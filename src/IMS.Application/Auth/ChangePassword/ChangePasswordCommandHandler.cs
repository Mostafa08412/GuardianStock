using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {

        private readonly IIdentityService _identityService;
        private readonly ICurrentUser _currentUser;

        public ChangePasswordCommandHandler(IIdentityService identityService, ICurrentUser currentUser)
        {
            _identityService = identityService;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.ChangePasswordAsync(Guid.Parse(_currentUser.UserId), request.CurrentPassword, request.NewPassword, request.ConfirmNewPassword);
        }
    }
}