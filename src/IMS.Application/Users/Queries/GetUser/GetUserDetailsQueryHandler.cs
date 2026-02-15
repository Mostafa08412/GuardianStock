using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Queries.GetUser;

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, Result<UserDetailsDto>>
{
    private readonly IIdentityService _identityService;

    public GetUserDetailsQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDetailsDto>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUserDetailsAsync(request.UserId, cancellationToken);

        if (result.IsFailure)
        {
            return Result<UserDetailsDto>.Failure(ApplicationErrors.UserErrors.NotFound(request.UserId));
        }

        return result;
    }
}
