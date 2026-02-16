using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.ForgetPassword
{
    public class ForgetPasswordCommandHandler : IRequestHandler<ForgetPasswordCommand, Result>
    {
        private readonly IIdentityService _identityService;
        private readonly IBackgroundJobWorker _backgroundJobWorker;

        public ForgetPasswordCommandHandler(IIdentityService identityService, IBackgroundJobWorker backgroundJobWorker)
        {
            _identityService = identityService;
            _backgroundJobWorker = backgroundJobWorker;
        }

        public async Task<Result> Handle(ForgetPasswordCommand request, CancellationToken cancellationToken)
        {
            var otpResult = await _identityService.GenerateResetPasswordOTP(request.EmailAddress, cancellationToken);

            if (otpResult.IsFailure)
            {
                return Result.Failure(otpResult.Errors);
            }



            _backgroundJobWorker.SendForgetPasswordEmail(new SendForgetPasswordEmail
            {
                To = request.EmailAddress,
                Name = otpResult.Value.fullName,
                EmailAddress = request.EmailAddress,
                Otp = otpResult.Value!.otp
            });

            return Result.Success();
        }
    }
}
