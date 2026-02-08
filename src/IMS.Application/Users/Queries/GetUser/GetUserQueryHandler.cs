using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Queries.GetUser;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IIdentityService _identityService;

    public GetUserQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var result = await _identityService.GetUserDetailsAsync(request.UserId, cancellationToken);
        
        if (result.IsFailure) 
        {
             return Result<UserDto>.Failure(ApplicationErrors.UserErrors.NotFound(request.UserId));
        }

        return result;
    }
}
