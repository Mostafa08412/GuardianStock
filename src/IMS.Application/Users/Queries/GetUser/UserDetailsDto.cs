namespace IMS.Application.Users.Queries.GetUser;

public record UserDetailsDto(
Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Role,
    bool IsActive,
    bool EmailConfirmed,
    DateTime? LastLoginDate,
    DateTime? LockoutEnd);
