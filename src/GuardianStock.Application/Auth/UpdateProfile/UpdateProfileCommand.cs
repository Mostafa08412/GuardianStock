using GuardianStock.Application.Contracts.Identity;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.UpdateProfile
{
    public record UpdateProfileCommand : IRequest<Result<IdentityUserDto>>
    {
        public string FirstName { get; init; }

        public string LastName { get; init; }
    }
}
