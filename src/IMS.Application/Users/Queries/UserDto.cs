namespace IMS.Application.Users.Queries;

public record UserDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Role,
    bool IsActive,
    bool EmailConfirmed,
    DateTime? LastLoginDate,
    DateTime? LockoutEnd);
