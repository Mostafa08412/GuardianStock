using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Domain.Core.Errors
{
    public static partial class Errors
    {


        public static class UserErrors
        {
            public static Error IdIsRequired => new Error("User_IdIsRequired__UserId", "User Id is required", ErrorType.Validation);
            public static Error FirstNameIsRequired => new Error("User_FirstNameIsRequired__FirstName", "First name is required", ErrorType.Validation);
            public static Error LastNameIsRequired => new Error("User_LastNameIsRequired__LastName", "Last name is required", ErrorType.Validation);
            public static Error UsernameIsRequired => new Error("User_UsernameIsRequired__Username", "Username is required", ErrorType.Validation);
            public static Error EmailIsRequired => new Error("User_EmailIsRequired__Email", "Email is required", ErrorType.Validation);
            public static Error InvalidEmail => new Error("User_InvalidEmail__Email", "Invalid email format", ErrorType.Validation);
            public static Error EmailTooLong => new Error("User_EmailTooLong__Email", "Email must not exceed 256 characters", ErrorType.Validation);
            public static Error FirstNameTooLong => new Error("User_FirstNameTooLong__FirstName", "First name must not exceed 100 characters", ErrorType.Validation);
            public static Error LastNameTooLong => new Error("User_LastNameTooLong__LastName", "Last name must not exceed 100 characters", ErrorType.Validation);
            public static Error InvalidRole => new Error("User_InvalidRole__Role", "Role must be Admin, Manager, or Staff", ErrorType.Validation);
            public static Error RoleIsRequired => new Error("User_RoleIsRequired__Role", "Role is required", ErrorType.Validation);
            public static Error UserNotFound => new Error("User_NotFound", "User not found", ErrorType.NotFound);
        }
    }
}
