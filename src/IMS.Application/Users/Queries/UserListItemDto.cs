namespace IMS.Application.Users.Queries;

public record UserListItemDto(
    string Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    bool IsActive,
    DateTime? LastLoginDate);
