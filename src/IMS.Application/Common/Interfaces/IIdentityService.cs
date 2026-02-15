using IMS.Application.Common.Models;
using IMS.Application.Contracts.Identity;
using IMS.Application.Users.Queries.GetUser;
using IMS.Application.Users.Queries.ListUsers;
using IMS.Domain.Core.Primitives.Result;

public interface IIdentityService
{
    #region User Roles Management Methods

    Task<Result> AddRoleToUserAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result> AddRolesToUserAsync(
        string userId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveRoleFromUserAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    #endregion

    #region User Existence Validation Methods

    Task<bool> EnsureEmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    #endregion

    #region Get User Methods

    Task<Result<IdentityUserDto>> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetUsersEmailsByRoleAsync(
        string role,
        CancellationToken cancellationToken = default);

    Task<PaginatedList<UserListItemDto>> ListUsersAsync(
        string? searchTerm,
        string? role,
        bool? isActive,
        string? SortBy,
        bool SortDescending,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<Result<UserDetailsDto>> GetUserDetailsAsync(
        string userId,
        CancellationToken cancellationToken = default);

    #endregion

    #region User Management Methods

    Task<Result> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default,
        string role = "Staff");

    Task<Result> LockUserAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UnlockUserAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateByRefreshTokenAsync(
     string refreshToken,
     CancellationToken cancellationToken = default);



    Task<Result> RevokeActiveRefreshTokenAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        string confirmNewPassword,
        CancellationToken cancellationToken = default);

    Task<Result> UpdateUserAsync(
        string userId,
        string firstName,
        string lastName,
        string role,
        CancellationToken cancellationToken = default);

    #endregion
}
