using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Models;
using MediatR;

namespace IMS.Application.Users.Queries.ListUsers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, PaginatedList<UserListItemDto>>
{
    private readonly IIdentityService _identityService;

    public ListUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<PaginatedList<UserListItemDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.ListUsersAsync(
            request.SearchTerm,
            request.Role,
            request.IsActive,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }
}
