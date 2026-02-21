using GuardianStock.Application.Auth.Common;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Users;
using MediatR;

namespace GuardianStock.Application.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IIdentityService _identityService;

        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(IIdentityService identityService, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var createUserResult = await _identityService.CreateUserAsync(request.FirstName, request.LastName, request.Email, request.Password, cancellationToken);

            if (createUserResult.IsFailure)
                return Result.Failure(createUserResult.Errors);

            var createDomainUserResult = User.Create(createUserResult.Value!.Id, createUserResult.Value.FirstName, createUserResult.Value.LastName, createUserResult.Value.Email, createUserResult.Value.Email);

            if (createDomainUserResult.IsFailure)
                return Result<AuthenticationResponse>.Failure(createDomainUserResult.Errors);

            _unitOfWork.Users.Add(createDomainUserResult.Value!);


            return createUserResult;
        }
    }
}
