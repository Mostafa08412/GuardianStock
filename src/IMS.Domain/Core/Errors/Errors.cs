using IMS.Domain.Core.Primitives;

namespace IMS.Domain.Core.Errors
{
    public static partial class Errors
    {

        public static class UserErrors
        {
            public static Error IdIsRequired => new Error("User.IdIsRequired", "User Id is required", ErrorType.Validation);
            public static Error FirstNameIsRequired => new Error("User.FirstNameIsRequired", "First name is required", ErrorType.Validation);
            public static Error LastNameIsRequired => new Error("User.LastNameIsRequired", "Last name is required", ErrorType.Validation);
            public static Error UsernameIsRequired => new Error("User.UsernameIsRequired", "Username is required", ErrorType.Validation);
            public static Error EmailIsRequired => new Error("User.EmailIsRequired", "Email is required", ErrorType.Validation);
        }
        public static class IdentityErrors
        {


            // ----- User Not Found -----
            public static Error UserNotFoundByUsername(string username) =>
                new("Identity.UserNotFound.Username", $"No user was found with username '{username}'.", ErrorType.NotFound);

            public static Error UserNotFoundByEmail(string email) =>
                new("Identity.UserNotFound.Email", $"No user was found with email '{email}'.", ErrorType.NotFound);

            public static Error UserNotFoundById(string userId) =>
                new("Identity.UserNotFound.Id", $"No user was found with id '{userId}'.", ErrorType.NotFound);

            public static Error UserNotFound() =>
                new("Identity.UserNotFound", "The specified user does not exist.", ErrorType.NotFound);

            // ----- Credentials -----

            public static Error InvalidGoogleIdToken =>
             new("Identity.InvalidGoogleClientId", "The provided Google ID token is invalid.", ErrorType.IdentityError);

            public static Error InvalidCredentials =>
                new("Identity.InvalidCredentials", "The provided credentials are invalid.", ErrorType.IdentityError);

            public static Error InvalidPassword =>
            new("Identity.InvalidPassword", "The provided password is invalid.", ErrorType.IdentityError);

            // ----- User Already Exists -----
            public static Error UserAlreadyExists(string email) =>
                new("Identity.UserAlreadyExists", $"A user with the email '{email}' already exists.", ErrorType.Conflict);

            public static Error UserAlreadyExists() =>
                new("Identity.UserAlreadyExists", "A user with this email already exists.", ErrorType.Conflict);

            public static Error UsernameAlreadyExists() =>
                new("Identity.UsernameAlreadyExists", "The username is already taken.", ErrorType.Conflict);


            public static Error EmailAlreadyExists =>
                new("Identity.EmailAlreadyExists", "The email is already taken.", ErrorType.Conflict);

            // ----- Invalid Input -----
            public static Error InvalidEmail =>
                new("Identity.InvalidEmail", "The email is invalid.", ErrorType.Validation);

            public static Error InvalidUsername =>
                new("Identity.InvalidUsername", "The username is invalid.", ErrorType.Validation);

            // ----- Password -----
            public static Error WeakPassword() =>
                new("Identity.WeakPassword", "The provided password does not meet the security requirements.", ErrorType.Validation);

            public static Error WeakPassword(string message) =>
                new("Identity.WeakPassword", message, ErrorType.Validation);

            public static Error PasswordReuseNotAllowed() =>
                new("Identity.PasswordReuseNotAllowed", "You cannot reuse a previous password.", ErrorType.Validation);

            // ----- Lockout / Concurrency -----
            public static Error UserLockout =>
                new("Identity.UserLockedOut", "The user account is locked.", ErrorType.AccessDenied);

            // ----- Roles -----
            public static Error RoleNotFound(string role) =>
                new("Identity.RoleNotFound", $"The role '{role}' does not exist.", ErrorType.NotFound);

            public static Error UserAlreadyInRole(string role) =>
                new("Identity.UserAlreadyInRole", $"The user is already in the role '{role}'.", ErrorType.Conflict);

            public static Error UserNotInRole(string role) =>
                new("Identity.UserNotInRole", $"The user is not in the role '{role}'.", ErrorType.NotFound);

            // ----- Tokens -----
            public static Error MissingToken => new(
                "Identity.MissingToken",
                "Token is missing. Authorization header is required.",
                ErrorType.IdentityError
            );

            public static Error InvalidToken => new(
                "Identity.InvalidToken",
                "Token is invalid or malformed.",
                ErrorType.IdentityError
            );

            public static Error ExpiredToken => new(
                "Identity.ExpiredToken",
                "Token has expired. Please refresh or login again.",
                ErrorType.IdentityError
            );

            public static Error ForbiddenAccess => new(
                "Identity.Forbidden",
                "Access denied. You do not have permission to access this resource.",
                ErrorType.AccessDenied
            );


            public static Error RecoveryCodeRedemptionFailed() =>
                new("Identity.RecoveryCodeRedemptionFailed", "The recovery code is invalid.", ErrorType.Validation);



            // ----- Fallback -----
            public static Error Unknown(string description) =>
                new("Identity.Unknown", description, ErrorType.Failure);
        }
    }
}
