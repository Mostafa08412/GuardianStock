using IMS.Application.Common.Interfaces;
using IMS.Application.Contracts.Identity;
using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.UpdateProfile
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

            var userId = Guid.Parse(_currentUser.UserId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

            if (user is null)
                return Result<IdentityUserDto>.Failure(Errors.UserErrors.UserNotFound);

            var updateProfileResult = user.UpdateProfile(request.FirstName, request.LastName);

            if (updateProfileResult.IsFailure)
                return Result<IdentityUserDto>.Failure(updateProfileResult.Errors);

            var updateIdentityResult = await _identityService.UpdateUserProfileAsync(Guid.Parse(_currentUser.UserId), request.FirstName, request.LastName, cancellationToken);

            if (updateIdentityResult.IsFailure)
                return Result<IdentityUserDto>.Failure(updateIdentityResult.Errors);


            return updateIdentityResult;
        }
    }
}
