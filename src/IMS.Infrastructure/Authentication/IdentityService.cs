using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Models;
using IMS.Application.Contracts.Identity;
using IMS.Application.Users.Queries.GetUser;
using IMS.Application.Users.Queries.ListUsers;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Enums;
using IMS.Infrastructure.Extensions;
using IMS.Infrastructure.Persistence;
using IMS.Infrastructure.Persistence.Identity;
using IMS.Infrastructure.Tokens;
using IMS.Infrastructure.Tokens.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RefreshToken = IMS.Infrastructure.Tokens.RefreshToken;

namespace IMS.Infrastructure.Authentication
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly TokenSettings _tokenOptions;
        private readonly IDateTime _dateTime;

        public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ITokenService tokenService, IOptions<TokenSettings> tokenSettings, IDateTime dateTime)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
            _tokenOptions = tokenSettings.Value;
            _dateTime = dateTime;
        }


        private static IdentityUserDto MapToDto(ApplicationUser user, IEnumerable<string> roles)
        {
            return new IdentityUserDto
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles
            };
        }

        private async Task<Result<ApplicationUser>> ValidateUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if ((user is null))
            {
                return Result<ApplicationUser>.Failure(Errors.Identity.UserNotFoundById(userId));
            }
            return Result<ApplicationUser>.Success(user);
        }

        private async Task<Result<ApplicationUser>> ValidateUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if ((user is null))
            {
                return Result<ApplicationUser>.Failure(Errors.Identity.UserNotFoundByEmail(email));
            }
            return Result<ApplicationUser>.Success(user);
        }

        public async Task<Result> AddRoleToUserAsync(string userId, string role, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return Result.Failure(Errors.Identity.RoleNotFound(role));

            var identityResult = await _userManager.AddToRoleAsync(userResult.Value!, role);
            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }

        public async Task<Result> AddRolesToUserAsync(string userId, IEnumerable<string> roles, CancellationToken cancellationToken)
        {
            var appUserResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!appUserResult.IsSuccess)
                return Result.Failure(appUserResult.Errors);

            foreach (var role in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                    return Result.Failure(Errors.Identity.RoleNotFound(role));
            }

            var identityResult = await _userManager.AddToRolesAsync(appUserResult.Value!, roles);

            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }

        public async Task<bool> EnsureEmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null;
        }

        public async Task<Result<IdentityUserDto>> GetUserByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var result = await ValidateUserByIdAsync(userId, cancellationToken);

            if (result.IsSuccess == false)
            {
                return Result<IdentityUserDto>.Failure(result.Errors);
            }
            var userRoles = await _userManager.GetRolesAsync(result.Value!);

            var appUser = MapToDto(result.Value!, userRoles);

            return Result<IdentityUserDto>.Success(appUser);

        }

        public async Task<Result<IdentityUserDto>> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var result = await ValidateUserByEmail(email, cancellationToken);

            if (result.IsSuccess == false)
            {
                return Result<IdentityUserDto>.Failure(result.Errors);
            }
            var userRoles = await _userManager.GetRolesAsync(result.Value!);

            var appUser = MapToDto(result.Value!, userRoles);

            return Result<IdentityUserDto>.Success(appUser);
        }

        public async Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result<IEnumerable<string>>.Failure(userResult.Errors);

            var roles = await _userManager.GetRolesAsync(userResult.Value!);
            return Result<IEnumerable<string>>.Success(roles);
        }

        public async Task<Result> RemoveRoleFromUserAsync(string userId, string role, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return Result.Failure(Errors.Identity.RoleNotFound(role));

            var identityResult = await _userManager.RemoveFromRoleAsync(userResult.Value!, role);
            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }

        public async Task<Result> CreateUserAsync(string firstName, string lastName, string email, string password, CancellationToken cancellationToken, string role = Roles.Staff)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
                return Result.Failure(Errors.Identity.EmailAlreadyExists);

            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email

            };

            var identityResult = await _userManager.CreateAsync(applicationUser, password);


            if (!identityResult.Succeeded)

                return identityResult.ToResult();


            var addToRoleResult = await _userManager.AddToRoleAsync(applicationUser, role);

            if (!addToRoleResult.Succeeded)

                return addToRoleResult.ToResult();

            return Result.Success();
        }

        public async Task<Result> LockUser(string userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);

            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var setResult = await _userManager.SetLockoutEndDateAsync(userResult.Value!, _dateTime.UTCNow.AddYears(2));

            if (!setResult.Succeeded)
                return Result.Failure(userResult.Errors);

            return Result.Success();
        }

        public async Task<Result> UnlockUser(string userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);

            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var setResult = await _userManager.SetLockoutEndDateAsync(userResult.Value!, null);

            if (!setResult.Succeeded)
                return Result.Failure(userResult.Errors);

            return Result.Success();

        }

        public async Task<Result<AuthenticationResult>> AuthenticateAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userManager.Users
                .Where(X => X.Email == email)
                .Include(X => X.RefreshTokens)
                .FirstOrDefaultAsync(cancellationToken);

            if (user == null)
                return Result<AuthenticationResult>.Failure(Errors.Identity.InvalidCredentials);


            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);


            if (!result.Succeeded && !result.IsLockedOut)
                return Result<AuthenticationResult>.Failure(Errors.Identity.InvalidCredentials);

            if (!result.Succeeded && result.IsLockedOut)
                return Result<AuthenticationResult>.Failure(Errors.Identity.UserLockout);


            var userRoles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles);


            user.LastLoginDate = _dateTime.UTCNow;
            await _userManager.UpdateAsync(user);

            RefreshToken refreshToken;

            if (!user.RefreshTokens.Any(X => X.IsActive))
            {
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                refreshToken = RefreshToken.Create(user.Id, newRefreshToken.Item1, newRefreshToken.Item2);
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            else
            {
                refreshToken = user.RefreshTokens.First(X => X.IsActive);
            }

            var authResult = new AuthenticationResult
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles,
                AccessToken = accessToken.Item1,
                AccessTokenExpiresAt = accessToken.Item2,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAt = refreshToken.ExpiresAtUTC
            };


            return Result<AuthenticationResult>.Success(authResult);


        }

        public async Task<Result<AuthenticationResult>> AuthenticateAsync(string refreshToken, CancellationToken cancellationToken)
        {


            if (string.IsNullOrEmpty(refreshToken))
                return Result<AuthenticationResult>.Failure(Errors.Identity.InvalidToken);

            var user = await _userManager.Users
                .AsNoTracking()
                .Where(X => X.RefreshTokens
                .Any(X => X.Token == refreshToken))
                .FirstOrDefaultAsync(cancellationToken);



            if (user is null)
                return Result<AuthenticationResult>.Failure(Errors.Identity.InvalidToken);

            var RefreshToken = user.RefreshTokens.First(x => x.Token == refreshToken);

            if (!RefreshToken.IsActive)
                return Result<AuthenticationResult>.Failure(Errors.Identity.InvalidToken);


            var userRoles = await _userManager.GetRolesAsync(user);

            var accessToken = _tokenService.GenerateAccessToken(user, userRoles);


            var authResult = new AuthenticationResult
            {
                UserId = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = userRoles,
                AccessToken = accessToken.Item1,
                AccessTokenExpiresAt = accessToken.Item2,
                RefreshToken = RefreshToken.Token,
                RefreshTokenExpiresAt = RefreshToken.ExpiresAtUTC
            };

            return Result<AuthenticationResult>.Success(authResult);
        }

        public async Task<Result> RevokeActiveRefreshToken(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(Errors.Identity.UserNotFoundById(userId));

            var user = await _userManager.Users
            .Where(X => X.Id == userId)
             .Include(X => X.RefreshTokens)
            .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Result.Failure(Errors.Identity.UserNotFoundById(userId));

            if (user.RefreshTokens.Any(X => X.IsActive))
            {
                var refreshToken = user.RefreshTokens.First(X => X.IsActive);
                refreshToken.Revoke(_dateTime.UTCNow);
                await _userManager.UpdateAsync(user);
            }




            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword, string confirmNewPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure(Errors.Identity.UserNotFound());

            var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return identityResult.ToResult();



        }

        public async Task<IEnumerable<string>> GetUsersEmailsByRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            var r = await _context.Roles.FirstOrDefaultAsync(X => X.NormalizedName == role.ToUpper(), cancellationToken);


            var userEmails = await (from user in _context.Users.AsNoTracking()
                                    where user.EmailConfirmed
                                    join userRole in _context.UserRoles.AsNoTracking().Where(X => X.RoleId == r!.Id)
                                    on user.Id equals userRole.UserId
                                    select user.Email).ToListAsync(cancellationToken);

            return userEmails.ToArray();
        }

        public async Task<PaginatedList<UserListItemDto>> ListUsersAsync(
            string? searchTerm,
            string? role,
            bool? isActive,
            string? SortBy,
            bool SortDescending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var usersQuery = _userManager.Users.AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                usersQuery = usersQuery.Where(u =>
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.Email!.ToLower().Contains(term) ||
                    u.UserName!.ToLower().Contains(term));
            }

            if (isActive.HasValue)
            {
                usersQuery = usersQuery.Where(u =>
                    isActive.Value ? !u.LockoutEnd.HasValue || u.LockoutEnd <= DateTimeOffset.UtcNow
                                   : u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow);
            }


            var roles = _context.Roles.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(role))
            {
                var normalizedRole = role.ToUpper();
                roles = roles.Where(x => x.NormalizedName == normalizedRole);
            }

            var rolesDetails = from r in roles
                               join ur in _context.UserRoles.AsNoTracking()
                               on r.Id equals ur.RoleId
                               select new
                               {
                                   UserId = ur.UserId,
                                   RoleId = r.Id,
                                   RoleName = r.NormalizedName
                               };


            var usersWithRoles = from user in usersQuery.AsNoTracking()
                                 join userrole in rolesDetails.AsNoTracking()
                                 on user.Id equals userrole.UserId
                                 select new
                                 {
                                     User = user,
                                     RoleId = userrole.RoleId,
                                     RoleName = userrole.RoleName
                                 };

            if (!string.IsNullOrWhiteSpace(SortBy))
            {
                if (SortBy.ToLower() == "name")
                {
                    usersWithRoles = SortDescending ?
                    usersWithRoles.OrderByDescending(u => u.User.FirstName).ThenByDescending(u => u.User.LastName) :
                    usersWithRoles.OrderBy(u => u.User.FirstName).ThenBy(u => u.User.LastName);
                }
                if (SortBy.ToLower() == "role")
                {
                    usersWithRoles = SortDescending ?
                    usersWithRoles.OrderByDescending(u => u.RoleName) :
                    usersWithRoles.OrderBy(u => u.RoleName);
                }

                else if (SortBy.ToLower() == "email")
                {
                    usersWithRoles = SortDescending ?
                    usersWithRoles.OrderByDescending(u => u.User.Email) :
                    usersWithRoles.OrderBy(u => u.User.Email);
                }
            }


            var result = usersWithRoles.Select(X => new UserListItemDto
            {
                Email = X.User.Email,
                FirstName = X.User.FirstName,
                LastName = X.User.LastName,
                Role = X.RoleName,
                Id = X.User.Id,
                IsActive = X.User.LockoutEnd != default && X.User.LockoutEnd > DateTimeOffset.UtcNow ? false : true,
                LastLoginDate = X.User.LastLoginDate

            });




            var usersDtos = await result
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var totalCount = await result.CountAsync(cancellationToken);

            return new PaginatedList<UserListItemDto>(
                usersDtos,
                totalCount,
                pageNumber,
                pageSize);
        }

        public async Task<Result<UserDetailsDto>> GetUserDetailsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return Result<UserDetailsDto>.Failure(Errors.Identity.UserNotFoundById(userId));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Staff";

            var isActive = !user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.UtcNow;

            var userDto = new UserDetailsDto(
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email!,
                user.UserName!,
                role,
                isActive,
                user.EmailConfirmed,
                user.LastLoginDate,
                user.LockoutEnd?.DateTime);

            return Result<UserDetailsDto>.Success(userDto);
        }

        public async Task<Result> UpdateUserAsync(
            string userId,
            string firstName,
            string lastName,
            string role,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return Result.Failure(Errors.Identity.UserNotFoundById(userId));
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            role = role.ToUpper();

            //if (user.Email != email)
            //{
            //    var emailExists = await _userManager.FindByEmailAsync(email);
            //    if (emailExists != null && emailExists.Id != user.Id)
            //    {
            //        return Result.Failure(Errors.Identity.EmailAlreadyExists);
            //    }

            //    user.Email = email;
            //    user.UserName = email;
            //}

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return updateResult.ToResult();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            var currentRole = currentRoles.FirstOrDefault();

            if (currentRole != role)
            {
                if (currentRole != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    return Result.Failure(Errors.Identity.RoleNotFound(role));
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, role);

                if (!addRoleResult.Succeeded)
                {
                    return addRoleResult.ToResult();
                }
            }

            return Result.Success();
        }
    }
}