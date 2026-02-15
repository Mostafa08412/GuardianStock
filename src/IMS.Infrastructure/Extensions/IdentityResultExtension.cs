using IMS.Domain.Core.Primitives;
using IMS.Domain.Core.Primitives.Result;
using Microsoft.AspNetCore.Identity;
using static IMS.Domain.Core.Errors.Errors;

namespace IMS.Infrastructure.Extensions
{
    public static class IdentityResultExtensions
    {
        public static Error MapIdentityResultErrorToDefinedErrors(this IdentityError error)
        {
            return error.Code switch
            {

                // ===== Duplicate / Exists =====
                "DuplicateEmail" =>
                    IdentityErrors.EmailAlreadyExists,

                "DuplicateUserName" =>
                    IdentityErrors.UsernameAlreadyExists(),

                // ===== Invalid input =====
                "InvalidEmail" =>
                    IdentityErrors.InvalidEmail,

                "InvalidUserName" =>
                    IdentityErrors.InvalidUsername,
                "PasswordMismatch" =>
                IdentityErrors.InvalidPassword,

                // ===== Password =====
                "PasswordTooShort" or
                "PasswordRequiresDigit" or
                "PasswordRequiresLower" or
                "PasswordRequiresUpper" or
                "PasswordRequiresNonAlphanumeric" =>
                    IdentityErrors.WeakPassword(error.Description),

                "PasswordReuseNotAllowed" =>
                    IdentityErrors.PasswordReuseNotAllowed(),

                // ===== Security / Tokens =====
                "InvalidToken" =>
                    IdentityErrors.InvalidToken,

                "RecoveryCodeRedemptionFailed" =>
                    IdentityErrors.RecoveryCodeRedemptionFailed(),


                // ===== Fallback =====
                _ =>
                    IdentityErrors.Unknown(error.Description)
            };
        }

        public static Result<T> ToResult<T>(this IdentityResult result) where T : class
        {
            if (result.Succeeded)
            {
                return Result<T>.Success(null);
            }

            var errors = result.Errors.Select(e => e.MapIdentityResultErrorToDefinedErrors());
            return Result<T>.Failure(errors);
        }

        public static Result ToResult(this IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
            {
                return Result.Success();
            }

            var errors = identityResult.Errors
                .Select(e => e.MapIdentityResultErrorToDefinedErrors());

            return Result.Failure(errors);
        }
    }
}
