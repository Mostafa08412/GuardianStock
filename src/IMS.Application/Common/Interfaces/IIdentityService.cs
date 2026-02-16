using IMS.Application.Common.Models;
using IMS.Application.Contracts.Identity;
using IMS.Application.Users.Queries.GetUser;
using IMS.Application.Users.Queries.ListUsers;
using IMS.Domain.Core.Primitives.Result;

public interface IIdentityService
{
    #region User Roles Management Methods

    Task<Result> AddRoleToUserAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result> AddRolesToUserAsync(
        Guid userId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveRoleFromUserAsync(
        Guid userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    #endregion

    #region User Existence Validation Methods

    Task<bool> EnsureEmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    #endregion

    #region Get User Methods

    Task<Result<IdentityUserDto>> GetUserByIdAsync(
        Guid userId,
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
        Guid userId,
        CancellationToken cancellationToken = default);

    #endregion

    #region User Management Methods

    Task<Result<IdentityUserDto>> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default,
        string role = "Staff");

    Task<Result> LockUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result> UnlockUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateByRefreshTokenAsync(
     string refreshToken,
     CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateByGoogleTokenAsync(
     string googleTokenId,
     CancellationToken cancellationToken = default);

    Task<Result> RevokeActiveRefreshTokenAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        string confirmNewPassword,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> UpdateUserAsync(
        Guid userId,
        string firstName,
        string lastName,
        string? role,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> UpdateUserProfileAsync(
        Guid userId,
        string firstName,
        string lastName,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> CreateUserByGoogleTokenAsync(
        string googleTokenId
        , CancellationToken cancellationToken = default);


    Task<Result<(string otp, string fullName)>> GenerateResetPasswordOTP(
    string email,
    CancellationToken cancellationToken = default);



    Task<Result<string>> VerifyResetPasswordOTP(
        string email,
        string otp,
        CancellationToken cancellationToken = default);

    Task<Result> ResetPasswordAsync(
     string email,
     string resetToken,
     string newPassword,
     CancellationToken cancellationToken = default);

    #endregion
}
