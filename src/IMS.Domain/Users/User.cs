using IMS.Domain.Abstractions;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;

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
        private User()
        {
        }

        public static Result<User> Create(string id, string firstName, string lastName, string username, string email)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<User>.Failure(Errors.UserErrors.IdIsRequired);
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                return Result<User>.Failure(Errors.UserErrors.FirstNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                return Result<User>.Failure(Errors.UserErrors.LastNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return Result<User>.Failure(Errors.UserErrors.UsernameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return Result<User>.Failure(Errors.UserErrors.EmailIsRequired);
            }

            return Result<User>.Success(new User(id, firstName, lastName, username, email));
        }

        public static class Errors
        {
            public static class UserErrors
            {
                public static Error IdIsRequired => new Error("User.IdIsRequired", "User Id is required", ErrorType.Validation);
                public static Error FirstNameIsRequired => new Error("User.FirstNameIsRequired", "First name is required", ErrorType.Validation);
                public static Error LastNameIsRequired => new Error("User.LastNameIsRequired", "Last name is required", ErrorType.Validation);
                public static Error UsernameIsRequired => new Error("User.UsernameIsRequired", "Username is required", ErrorType.Validation);
                public static Error EmailIsRequired => new Error("User.EmailIsRequired", "Email is required", ErrorType.Validation);
            }
        }
    }
}
