using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.Core.Errors
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
            public static Error InvalidEmail => new Error("User.InvalidEmail-Email", "Invalid email format", ErrorType.Validation);
            public static Error EmailTooLong => new Error("User.EmailTooLong-Email", "Email must not exceed 256 characters", ErrorType.Validation);
            public static Error FirstNameTooLong => new Error("User.FirstNameTooLong", "First name must not exceed 100 characters", ErrorType.Validation);
            public static Error LastNameTooLong => new Error("User.LastNameTooLong", "Last name must not exceed 100 characters", ErrorType.Validation);
            public static Error InvalidRole => new Error("User.InvalidRole-Role", "Role must be Admin, Manager, or Staff", ErrorType.Validation);
            public static Error RoleIsRequired => new Error("User.RoleIsRequired-Role", "Role is required", ErrorType.Validation);
            public static Error UserNotFound => new Error("User.NotFound", "User not found", ErrorType.NotFound);
        }
    }
}
