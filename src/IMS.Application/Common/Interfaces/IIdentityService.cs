using IMS.Application.Contracts.Identity;
using IMS.Domain.Core.Primitives.Result;

public interface IIdentityService
{
    Task<Result<AuthenticationResult>> AuthenticateAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result<AuthenticationResult>> AuthenticateAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RevokeActiveRefreshToken(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetUserByIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result<IdentityUserDto>> GetUserByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetUsersEmailsByRoleAsync(
    string role,
    CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> CreateUserAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<Result> ChangePasswordAsync(
        string userId,
        string currentPassword,
        string newPassword,
        string confirmNewPassword,
        CancellationToken cancellationToken = default);

    Task<Result> AddRoleToUserAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result> RemoveRoleFromUserAsync(
        string userId,
        string role,
        CancellationToken cancellationToken = default);

    Task<Result> AddRolesToUserAsync(
        string userId,
        IEnumerable<string> roles,
        CancellationToken cancellationToken = default);

    Task<bool> EnsureEmailExistsAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<Result> LockUser(
        string userId,
        CancellationToken cancellationToken = default);

    Task<Result> UnlockUser(
        string userId,
        CancellationToken cancellationToken = default);
}