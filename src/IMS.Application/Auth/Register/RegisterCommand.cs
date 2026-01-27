using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.Register
{
    public record RegisterCommand : IRequest<Result>
    {
        public string Email { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Password { get; init; }

    }
}
