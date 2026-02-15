using IMS.Application.Auth.Common;
using IMS.Application.Common.Mappers;
using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Users;
using MediatR;

namespace IMS.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommand, Result<AuthenticationResponse>>
    {
        private readonly IIdentityService _identityService;

        private readonly IUnitOfWork _unitOfWork;


        public GoogleLoginCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AuthenticationResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        {

            var googleAuthenticationResult = await _identityService.AuthenticateByGoogleTokenAsync(request.IdToken, cancellationToken);


            if (googleAuthenticationResult.IsFailure && googleAuthenticationResult.Errors.Any(X => X.Code == Errors.IdentityErrors.UserNotFoundByEmail("").Code))
            {

                var createUserResult = await _identityService.CreateUserByGoogleTokenAsync(request.IdToken, cancellationToken);

                if (createUserResult.IsFailure)
                    return Result<AuthenticationResponse>.Failure(createUserResult.Errors);

                var createDomainUserResult = User.Create(createUserResult.Value!.Id, createUserResult.Value.FirstName, createUserResult.Value.LastName, createUserResult.Value.Email, createUserResult.Value.Email);

                if (createDomainUserResult.IsFailure)
                    return Result<AuthenticationResponse>.Failure(createDomainUserResult.Errors);

                _unitOfWork.Users.Add(createDomainUserResult.Value!);

                googleAuthenticationResult = await _identityService.AuthenticateByGoogleTokenAsync(request.IdToken, cancellationToken);

            }
            else if (googleAuthenticationResult.IsFailure)

                return Result<AuthenticationResponse>.Failure(googleAuthenticationResult.Errors);


            return Result<AuthenticationResponse>.Success((googleAuthenticationResult.Value!).MapToAuthenticationResponse());
        }



    }


}
