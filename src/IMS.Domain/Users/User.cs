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

        private User(Guid id, string firstName, string lastName, string username, string email) : base(id)
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

        public static Result<User> Create(Guid id, string firstName, string lastName, string username, string email)
        {
            var errors = new List<Error>();

            if (id == Guid.Empty)
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

        public Result UpdateFirstName(string firstName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                return Result.Failure(Errors.UserErrors.FirstNameIsRequired);
            }

            if (firstName.Length > 100)
            {
                return Result.Failure(Errors.UserErrors.FirstNameTooLong);
            }

            FirstName = firstName;

            return Result.Success();
        }

        public Result UpdateLastName(string lastName)
        {
            if (string.IsNullOrWhiteSpace(lastName))
            {
                return Result.Failure(Errors.UserErrors.LastNameIsRequired);
            }

            if (lastName.Length > 100)
            {
                return Result.Failure(Errors.UserErrors.LastNameTooLong);
            }

            LastName = lastName;

            return Result.Success();
        }

        public Result UpdateProfile(string firstName, string lastName)
        {
            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(Errors.UserErrors.FirstNameIsRequired);
            }
            else if (firstName.Length > 100)
            {
                errors.Add(Errors.UserErrors.FirstNameTooLong);
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(Errors.UserErrors.LastNameIsRequired);
            }
            else if (lastName.Length > 100)
            {
                errors.Add(Errors.UserErrors.LastNameTooLong);
            }

            if (errors.Any())
            {
                return Result.Failure(errors);
            }

            FirstName = firstName;
            LastName = lastName;

            return Result.Success();
        }

    }
}
