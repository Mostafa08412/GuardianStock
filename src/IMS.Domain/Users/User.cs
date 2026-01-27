using IMS.Domain.Abstractions;

namespace IMS.Domain.Users
{
    public sealed class User : IUser
    {

        public string Id { get; private set; } // related to the application user.
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }

        private User(string id, string firstName, string lastName, string username, string email)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Email = email;
        }

        // for EF Core
        protected User()
        {

        }

        public static User Create(string id, string firstName, string lastName, string username, string email)
        {
            return new User(
            id,
            firstName,
           lastName,
           username,
           email);
        }
    }
}
