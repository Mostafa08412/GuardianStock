using IMS.Domain.Abstractions;
using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;

namespace IMS.Domain.Users
{
    public sealed class User : Aggregate, IUser
    {
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }

        private User(string id, string firstName, string lastName, string username, string email) : base(new Guid(id))
        {
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
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(id))
            {
                errors.Add(Errors.UserErrors.IdIsRequired);
            }

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(Errors.UserErrors.FirstNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(Errors.UserErrors.LastNameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                errors.Add(Errors.UserErrors.UsernameIsRequired);
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(Errors.UserErrors.EmailIsRequired);
            }

            if (errors.Any())
            {
                return Result<User>.Failure(errors);
            }

            return Result<User>.Success(new User(id, firstName, lastName, username, email));
        }


    }
}
