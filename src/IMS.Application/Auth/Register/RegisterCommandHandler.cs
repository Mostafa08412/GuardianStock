using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result>
    {
        private readonly IIdentityService _identityService;


        public RegisterCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

            var createUserResult = await _identityService.CreateUserAsync(request.FirstName, request.LastName, request.Email, request.Password);


            return createUserResult;
        }
    }
}
