namespace IMS.Application.Users.Queries.ListUsers;


public class UserListItemDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastLoginDate { get; set; }
}