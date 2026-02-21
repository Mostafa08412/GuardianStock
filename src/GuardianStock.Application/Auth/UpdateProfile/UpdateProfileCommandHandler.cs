using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.Contracts.Identity;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Auth.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Result<IdentityUserDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProfileCommandHandler(IIdentityService identityService, ICurrentUser currentUser, IUnitOfWork unitOfWork)
        {
            _identityService = identityService;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IdentityUserDto>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {

            var userId = _currentUser.UserId!.Value;
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                return Result<IdentityUserDto>.Failure(Errors.UserErrors.UserNotFound);

            var updateProfileResult = user.UpdateProfile(request.FirstName, request.LastName);

            if (updateProfileResult.IsFailure)
                return Result<IdentityUserDto>.Failure(updateProfileResult.Errors);

            var updateIdentityResult = await _identityService.UpdateUserProfileAsync(userId, request.FirstName, request.LastName, cancellationToken);

            if (updateIdentityResult.IsFailure)
                return Result<IdentityUserDto>.Failure(updateIdentityResult.Errors);


            return updateIdentityResult;
        }
    }
}
