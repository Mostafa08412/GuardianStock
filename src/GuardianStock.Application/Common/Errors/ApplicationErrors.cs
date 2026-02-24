using GuardianStock.Domain.Core.Primitives;

namespace GuardianStock.Application.Common.Errors;

public static class ApplicationErrors
{
    public static class IdentityErrors
    {
        // ----- User Search & Identification -----
        public static Error NotFound(string userId) =>
            new("Identity_UserNotFound", $"No account associated with User ID '{userId}' could be located.", ErrorType.NotFound);

        public static Error UserNotFoundById(string userId) =>
            new("Identity_UserNotFound_Id", $"The identifier '{userId}' does not match any existing user record.", ErrorType.NotFound);

        public static Error UserNotFoundByUsername(string username) =>
            new("Identity_UserNotFound_Username", $"The username '{username}' is not registered in our system.", ErrorType.NotFound);

        public static Error UserNotFoundByEmail(string email) =>
            new("Identity_UserNotFound_Email", $"No account found associated with the email address '{email}'.", ErrorType.NotFound);

        public static Error UserNotFound() =>
            new("Identity_UserNotFound", "The requested user profile does not exist.", ErrorType.NotFound);

        // ----- Authentication & Credentials -----
        public static Error InvalidCredentials =>
            new("Identity_InvalidCredentials", "Authentication failed. Please verify your username and password.", ErrorType.IdentityError);

        public static Error InvalidPassword =>
            new("Identity_InvalidPassword", "The password provided is incorrect. Please try again.", ErrorType.IdentityError);

        public static Error InvalidGoogleIdToken =>
            new("Identity_InvalidGoogleToken", "The Google authentication token is invalid or has expired.", ErrorType.IdentityError);

        // ----- Validation: Required Fields -----
        public static Error EmailIsRequired =>
            new("Identity_EmailIsRequired__EmailAddress", "Email address is required.", ErrorType.Validation);

        public static Error PasswordIsRequired =>
            new("Identity_PasswordIsRequired__Password", "Password is required.", ErrorType.Validation);

        public static Error CurrentPasswordIsRequired =>
            new("Identity_CurrentPasswordIsRequired__CurrentPassword", "Current password is required.", ErrorType.Validation);

        public static Error NewPasswordIsRequired =>
            new("Identity_NewPasswordIsRequired__NewPassword", "New password is required.", ErrorType.Validation);

        public static Error ConfirmNewPasswordIsRequired =>
            new("Identity_ConfirmNewPasswordIsRequired__ConfirmNewPassword", "Confirm new password is required.", ErrorType.Validation);

        public static Error ConfirmPasswordMismatch =>
            new("Identity_ConfirmPasswordMismatch__ConfirmNewPassword", "New password and confirm password do not match.", ErrorType.Validation);

        public static Error FirstNameIsRequired =>
            new("Identity_FirstNameIsRequired__FirstName", "First name is required.", ErrorType.Validation);

        public static Error LastNameIsRequired =>
            new("Identity_LastNameIsRequired__LastName", "Last name is required.", ErrorType.Validation);

        public static Error FirstNameTooLong =>
            new("Identity_FirstNameTooLong__FirstName", "First name must not exceed 100 characters.", ErrorType.Validation);

        public static Error LastNameTooLong =>
            new("Identity_LastNameTooLong__LastName", "Last name must not exceed 100 characters.", ErrorType.Validation);

        public static Error GoogleIdTokenIsRequired =>
            new("Identity_GoogleIdTokenIsRequired__IdToken", "Google ID token is required.", ErrorType.Validation);

        public static Error RefreshTokenIsRequired =>
            new("Identity_RefreshTokenIsRequired__RefreshToken", "Refresh token is required.", ErrorType.Validation);

        public static Error OtpIsRequired =>
            new("Identity_OtpIsRequired__Otp", "OTP code is required.", ErrorType.Validation);

        public static Error ResetPasswordTokenIsRequired =>
            new("Identity_ResetPasswordTokenIsRequired__ResetPasswordToken", "Reset password token is required.", ErrorType.Validation);

        // ----- Account Conflicts -----
        public static Error EmailAlreadyExists(string email) =>
            new("Identity_EmailAlreadyExists", $"The email address '{email}' is already linked to another account.", ErrorType.Conflict);


        public static Error EmailAlreadyExists() =>
            new("Identity_EmailAlreadyExists", "This email address is already registered. Please use a different email or sign in.", ErrorType.Conflict);

        public static Error UserAlreadyExists(string email) =>
            new("Identity_UserAlreadyExists", $"An active account already exists for '{email}'.", ErrorType.Conflict);

        public static Error UserAlreadyExists() =>
            new("Identity_UserAlreadyExists", "A user with these credentials already exists in the system.", ErrorType.Conflict);

        public static Error UsernameAlreadyExists() =>
            new("Identity_UsernameAlreadyExists", "This username is unavailable. Please choose another.", ErrorType.Conflict);

        // ----- Validation & Security Policy -----
        public static Error InvalidEmail =>
            new("Identity_InvalidEmail__EmailAddress", "The email format is invalid. Please enter a valid email address.", ErrorType.Validation);

        public static Error InvalidUsername =>
            new("Identity_InvalidUsername__Username", "The username contains invalid characters or does not meet the required length.", ErrorType.Validation);

        public static Error WeakPassword() =>
            new("Identity_WeakPassword__Password", "Password does not meet the minimum security requirements (complexity/length).", ErrorType.Validation);

        public static Error WeakPassword(string message) =>
            new("Identity_WeakPassword__Password", message, ErrorType.Validation);

        public static Error PasswordReuseNotAllowed() =>
            new("Identity_PasswordReuseNotAllowed__Password", "For security reasons, you cannot reuse a previously used password.", ErrorType.Validation);

        // ----- Status & Permissions -----
        public static Error ForbiddenAccess =>
            new("Identity_Forbidden", "Access Denied. You do not have the required permissions for this resource.", ErrorType.AccessDenied);

        public static Error UserLockout =>
            new("Identity_UserLockedOut", "This account has been temporarily locked due to multiple failed login attempts.", ErrorType.AccessDenied);

        public static Error RoleNotFound(string role) =>
            new("Identity_RoleNotFound", $"The specified security role '{role}' does not exist.", ErrorType.NotFound);

        public static Error UserAlreadyInRole(string role) =>
            new("Identity_UserAlreadyInRole", $"The user is already assigned to the '{role}' role.", ErrorType.Conflict);

        public static Error UserNotInRole(string role) =>
            new("Identity_UserNotInRole", $"The user does not possess the '{role}' role assignment.", ErrorType.NotFound);

        // ----- Token Management -----
        public static Error MissingToken =>
            new("Identity_MissingToken", "Authentication token is missing. Please provide a valid Authorization header.", ErrorType.IdentityError);

        public static Error InvalidToken =>
            new("Identity_InvalidToken", "The provided authentication token is malformed or invalid.", ErrorType.IdentityError);

        public static Error ExpiredToken =>
            new("Identity_ExpiredToken", "Your session has expired. Please log in again to continue.", ErrorType.IdentityError);

        public static Error RecoveryCodeRedemptionFailed() =>
            new("Identity_RecoveryCodeRedemptionFailed", "The recovery code provided is invalid or has already been used.", ErrorType.Validation);

        // ----- Password Reset -----
        public static Error OtpCooldown() =>
            new("Identity_OtpCooldown__Otp", "Please wait 30 seconds before requesting another OTP.", ErrorType.Validation);

        public static Error InvalidOtp =>
            new("Identity_InvalidOtp__Otp", "The OTP code entered is invalid or has expired.", ErrorType.Validation);

        public static Error InvalidResetToken =>
            new("Identity_InvalidResetToken__ResetPasswordToken", "The password reset token is invalid or has expired.", ErrorType.Validation);

        // ----- Action Failures -----
        public static Error UpdateFailed(string details) =>
            new("Identity_UpdateFailed", $"An error occurred while updating the user profile: {details}", ErrorType.Failure);

        public static Error RoleUpdateFailed(string details) =>
            new("Identity_RoleUpdateFailed", $"Failed to modify user role assignments: {details}", ErrorType.Failure);

        public static Error DeactivationFailed(string details) =>
            new("Identity_DeactivationFailed", $"Account deactivation process failed: {details}", ErrorType.Failure);

        public static Error ActivationFailed(string details) =>
            new("Identity_ActivationFailed", $"Account activation process failed: {details}", ErrorType.Failure);

        public static Error Unknown(string description) =>
            new("Identity_Unknown", $"An unexpected identity error occurred: {description}", ErrorType.Failure);
    }

    public static class CsvReader
    {
        public static Error InvalidFormat => new(
            "CSV_InvalidFormat", "Unsupported file format. Please upload a standard comma-separated values (.csv) file.", ErrorType.Failure);

        public static Error FileIsRequired => new(
            "CSV_FileRequired__File", "No file detected. Please select a valid CSV file to proceed.", ErrorType.Validation);

        public static Error FileNotFound => new(
            "CSV_FileNotFound", "The requested CSV file could not be located on the server.", ErrorType.NotFound);

        public static Error MissingHeaders => new(
            "CSV_MissingHeaders", "The uploaded file is missing mandatory header columns. Please check the template.", ErrorType.Failure);

        public static Error UnexpectedError => new(
            "CSV_UnexpectedError", "A critical error occurred while parsing the CSV data stream.", ErrorType.Failure);

        public static readonly Error PreviewExpired = new(
            "CSV_PreviewExpired", "The data preview session has expired. Please re-upload the file to continue.", ErrorType.NotFound);

        public static readonly Error DuplicateProduct = new(
            "CSV_DuplicateProduct", "Data integrity error: The file contains duplicate product entries.", ErrorType.Validation);

        public static readonly Error InvalidLowStockFormat = new(
            "CSV_InvalidLowStockFormat", "Format Error: The low stock threshold must be a valid non-negative integer.", ErrorType.Validation);

        public static Error InvalidInitialStock => new(
            "CSV_InvalidInitialStock", "Format Error: The initial stock quantity must be a valid non-negative integer.", ErrorType.Validation);

        public static Error LowStockTooLow(int minThreshold) => new(
            "CSV_LowStockTooLow", $"Invalid low stock threshold quantity (minimum is {minThreshold}).", ErrorType.Validation);

        public static Error InvalidCategory => new(
            "CSV_InvalidCategory", "The specified category does not exist in the system.", ErrorType.Validation);
    }

    public static class EmailErrors
    {
        public static Error SendFailed(string details) =>
            new("Email_SendFailed", $"Email delivery failed: {details}", ErrorType.Failure);
    }
}