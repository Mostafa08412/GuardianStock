using GuardianStock.Application.Auth.Common;
using GuardianStock.Application.Common.Errors;
using GuardianStock.Application.Common.Mappers;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Users;
using MediatR;

namespace GuardianStock.Application.Auth.GoogleLogin
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


            if (googleAuthenticationResult.IsFailure && googleAuthenticationResult.Errors.Any(X => X.Code == ApplicationErrors.IdentityErrors.UserNotFoundByEmail("").Code))
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
