using IMS.Application.Common.Models;
using MediatR;

namespace IMS.Application.Users.Queries.ListUsers;

public record ListUsersQuery(
    string? SearchTerm,
    string? Role,
    bool? IsActive,
    string? SortBy,
    bool SortDescending,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<UserListItemDto>>;
