using IMS.Application.Contracts.Identity;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.UpdateProfile
{
    public record UpdateProfileCommand : IRequest<Result<IdentityUserDto>>
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}
