using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateUserAsync(
            Guid.Parse(request.UserId),
            request.FirstName,
            request.LastName,
            request.Role,
            cancellationToken);

        if (result.IsFailure)

            return Result.Failure(result.Errors);


        return Result.Success();
    }
}
