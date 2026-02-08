using IMS.Domain.Core.Primitives;

namespace IMS.Application.Users;

public static class ApplicationErrors
{
    public static class UserErrors
    {
        public static Error NotFound(string userId) =>
            new Error("User.NotFound", $"User with ID '{userId}' was not found", ErrorType.NotFound);

        public static Error EmailAlreadyExists(string email) =>
            new Error("User.EmailAlreadyExists", $"Email '{email}' is already in use", ErrorType.Conflict);

        public static Error UpdateFailed(string details) =>
            new Error("User.UpdateFailed", $"Failed to update user: {details}", ErrorType.Failure);

        public static Error RoleUpdateFailed(string details) =>
            new Error("User.RoleUpdateFailed", $"Failed to update user role: {details}", ErrorType.Failure);

        public static Error DeactivationFailed(string details) =>
            new Error("User.DeactivationFailed", $"Failed to deactivate user: {details}", ErrorType.Failure);

        public static Error ActivationFailed(string details) =>
            new Error("User.ActivationFailed", $"Failed to activate user: {details}", ErrorType.Failure);
    }
}
