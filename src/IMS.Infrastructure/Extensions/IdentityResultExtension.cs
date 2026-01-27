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
                    Identity.EmailAlreadyExists,

                "DuplicateUserName" =>
                    Identity.UsernameAlreadyExists(),

                // ===== Invalid input =====
                "InvalidEmail" =>
                    Identity.InvalidEmail,

                "InvalidUserName" =>
                    Identity.InvalidUsername,
                "PasswordMismatch" =>
                Identity.InvalidPassword,

                // ===== Password =====
                "PasswordTooShort" or
                "PasswordRequiresDigit" or
                "PasswordRequiresLower" or
                "PasswordRequiresUpper" or
                "PasswordRequiresNonAlphanumeric" =>
                    Identity.WeakPassword(error.Description),

                "PasswordReuseNotAllowed" =>
                    Identity.PasswordReuseNotAllowed(),

                // ===== Security / Tokens =====
                "InvalidToken" =>
                    Identity.InvalidToken,

                "RecoveryCodeRedemptionFailed" =>
                    Identity.RecoveryCodeRedemptionFailed(),


                // ===== Fallback =====
                _ =>
                    Identity.Unknown(error.Description)
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
