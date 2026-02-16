using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;

        public ResetPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.ResetPasswordAsync(
                request.EmailAddress,
                request.ResetPasswordToken,
                request.NewPassword,
                cancellationToken);
        }
    }
}
