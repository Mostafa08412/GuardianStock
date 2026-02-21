using GuardianStock.Application.Common.Models;
using MediatR;

namespace GuardianStock.Application.Users.Queries.ListUsers;

public record ListUsersQuery(
    string? SearchTerm,
    string? Role,
    bool? IsActive,
    string? SortBy,
    bool SortDescending,
    int PageNumber = 1,
    int PageSize = 10) : IRequest<PaginatedList<UserListItemDto>>;
