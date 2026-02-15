using Google.Apis.Auth;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using RefreshToken = IMS.Infrastructure.Tokens.RefreshToken;

namespace IMS.Infrastructure.Authentication
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly TokenSettings _tokenOptions;
        private readonly IDateTime _dateTime;
        private readonly IConfiguration _configuration;

        public IdentityService(IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, ITokenService tokenService, IOptions<TokenSettings> tokenSettings, IDateTime dateTime)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
            _tokenOptions = tokenSettings.Value;
            _dateTime = dateTime;
            _configuration = configuration;
        }


        #region Private Helper Methods
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
        private async Task<Result<ApplicationUser>> ValidateUserByIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if ((user is null))
            {
                return Result<ApplicationUser>.Failure(Errors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }
            return Result<ApplicationUser>.Success(user);
        }

        private async Task<Result<ApplicationUser>> ValidateUserByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if ((user is null))
            {
                return Result<ApplicationUser>.Failure(Errors.IdentityErrors.UserNotFoundByEmail(email));
            }
            return Result<ApplicationUser>.Success(user);
        }
        #endregion

        #region User Roles Management Methods    
        public async Task<Result> AddRoleToUserAsync(Guid userId, string role, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return Result.Failure(Errors.IdentityErrors.RoleNotFound(role));

            var identityResult = await _userManager.AddToRoleAsync(userResult.Value!, role);
            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }
        public async Task<Result> AddRolesToUserAsync(Guid userId, IEnumerable<string> roles, CancellationToken cancellationToken)
        {
            var appUserResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!appUserResult.IsSuccess)
                return Result.Failure(appUserResult.Errors);

            foreach (var role in roles)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                    return Result.Failure(Errors.IdentityErrors.RoleNotFound(role));
            }

            var identityResult = await _userManager.AddToRolesAsync(appUserResult.Value!, roles);

            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }
        public async Task<Result> RemoveRoleFromUserAsync(Guid userId, string role, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var roleExists = await _roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return Result.Failure(Errors.IdentityErrors.RoleNotFound(role));

            var identityResult = await _userManager.RemoveFromRoleAsync(userResult.Value!, role);
            if (!identityResult.Succeeded)
                return identityResult.ToResult();

            return Result.Success();
        }
        public async Task<Result<IEnumerable<string>>> GetUserRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);
            if (!userResult.IsSuccess)
                return Result<IEnumerable<string>>.Failure(userResult.Errors);

            var roles = await _userManager.GetRolesAsync(userResult.Value!);
            return Result<IEnumerable<string>>.Success(roles);
        }


        #endregion

        #region User Existence Validation Methods
        public async Task<bool> EnsureEmailExistsAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user != null;
        }

        #endregion

        #region Get User Methods
        public async Task<Result<IdentityUserDto>> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
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

        public async Task<Result<UserDetailsDto>> GetUserDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<UserDetailsDto>.Failure(Errors.IdentityErrors.UserNotFoundById(userId.ToString()));
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
        #endregion

        #region User Management Methods

        public async Task<Result<IdentityUserDto>> CreateUserAsync(
            string firstName,
            string lastName,
            string email,
            string password,
            CancellationToken cancellationToken,
            string role = Roles.Staff)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null)
                return Result<IdentityUserDto>.Failure(Errors.IdentityErrors.EmailAlreadyExists);

            ApplicationUser applicationUser = new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email

            };

            var identityResult = await _userManager.CreateAsync(applicationUser, password);


            if (!identityResult.Succeeded)

                return identityResult.ToResult<IdentityUserDto>();


            var addToRoleResult = await _userManager.AddToRoleAsync(applicationUser, role);

            if (!addToRoleResult.Succeeded)

                return addToRoleResult.ToResult<IdentityUserDto>();


            var identityUser = MapToDto(applicationUser, new List<string> { role });

            return Result<IdentityUserDto>.Success(identityUser);
        }

        public async Task<Result> LockUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var userResult = await ValidateUserByIdAsync(userId, cancellationToken);

            if (!userResult.IsSuccess)
                return Result.Failure(userResult.Errors);

            var setResult = await _userManager.SetLockoutEndDateAsync(userResult.Value!, _dateTime.UTCNow.AddYears(2));

            if (!setResult.Succeeded)
                return Result.Failure(userResult.Errors);

            return Result.Success();
        }

        public async Task<Result> UnlockUserAsync(Guid userId, CancellationToken cancellationToken)
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
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.InvalidCredentials);


            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);


            if (!result.Succeeded && !result.IsLockedOut)
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.InvalidCredentials);

            if (!result.Succeeded && result.IsLockedOut)
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.UserLockout);


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

        public async Task<Result<AuthenticationResult>> AuthenticateByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {


            if (string.IsNullOrEmpty(refreshToken))
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.InvalidToken);

            var user = await _userManager.Users
                .AsNoTracking()
                .Where(X => X.RefreshTokens
                .Any(X => X.Token == refreshToken))
                .FirstOrDefaultAsync(cancellationToken);



            if (user is null)
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.InvalidToken);

            var RefreshToken = user.RefreshTokens.First(x => x.Token == refreshToken);

            if (!RefreshToken.IsActive)
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.InvalidToken);


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

        public async Task<Result> RevokeActiveRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            if (userId == Guid.Empty)
                return Result.Failure(Errors.IdentityErrors.UserNotFoundById(userId.ToString()));

            var user = await _userManager.Users
            .Where(X => X.Id == userId)
             .Include(X => X.RefreshTokens)
            .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return Result.Failure(Errors.IdentityErrors.UserNotFoundById(userId.ToString()));

            if (user.RefreshTokens.Any(X => X.IsActive))
            {
                var refreshToken = user.RefreshTokens.First(X => X.IsActive);
                refreshToken.Revoke(_dateTime.UTCNow);
                await _userManager.UpdateAsync(user);
            }




            return Result.Success();
        }
        public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string confirmNewPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
                return Result.Failure(Errors.IdentityErrors.UserNotFound());

            var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return identityResult.ToResult();



        }



        public async Task<Result<IdentityUserDto>> UpdateUserAsync(
            Guid userId,
            string firstName,
            string lastName,
            string? newRole,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<IdentityUserDto>.Failure(Errors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }

            user.FirstName = firstName;

            user.LastName = lastName;


            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return updateResult.ToResult<IdentityUserDto>();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var currentRole = currentRoles.FirstOrDefault();


            if (!string.IsNullOrWhiteSpace(newRole) && currentRole != newRole)
            {
                if (currentRole != null)
                {
                    await _userManager.RemoveFromRoleAsync(user, currentRole);
                }

                var roleExists = await _roleManager.RoleExistsAsync(newRole);

                if (!roleExists)
                {
                    return Result<IdentityUserDto>.Failure(Errors.IdentityErrors.RoleNotFound(newRole));
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, newRole);


                if (!addRoleResult.Succeeded)
                {
                    return addRoleResult.ToResult<IdentityUserDto>();
                }

                currentRoles.Clear();

                currentRoles.Add(newRole);
            }

            return Result<IdentityUserDto>.Success(MapToDto(user, currentRoles));
        }

        public async Task<Result<IdentityUserDto>> UpdateUserProfileAsync(
            Guid userId,
            string firstName,
            string lastName,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<IdentityUserDto>.Failure(Errors.IdentityErrors.UserNotFoundById(userId.ToString()));
            }

            user.FirstName = firstName;
            user.LastName = lastName;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return updateResult.ToResult<IdentityUserDto>();
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Result<IdentityUserDto>.Success(MapToDto(user, roles));
        }

        public async Task<Result<AuthenticationResult>> AuthenticateByGoogleTokenAsync(string googleTokenId, CancellationToken cancellationToken = default)
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new GoogleJsonWebSignature.ValidationSettings();

            validationSettings.Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] ?? string.Empty };


            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokenId, validationSettings);

                var user = await _userManager.FindByEmailAsync(payload.Email);


                if (user == null)
                    return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.UserNotFoundByEmail(payload.Email));


                if (user.LockoutEnd != null && user.LockoutEnd > _dateTime.UTCNow)
                    return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.UserLockout);

                var userRoles = await _userManager.GetRolesAsync(user);

                (string accessToken, DateTime expirationDate) = _tokenService.GenerateAccessToken(user, userRoles);

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
                    AccessToken = accessToken,
                    AccessTokenExpiresAt = expirationDate,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiresAt = refreshToken.ExpiresAtUTC
                };

                return Result<AuthenticationResult>.Success(authResult);
            }
            catch (Exception)
            {
                return Result<AuthenticationResult>.Failure(Errors.IdentityErrors.InvalidGoogleIdToken);
            }



        }


        public async Task<Result<IdentityUserDto>> CreateUserByGoogleTokenAsync(string googleTokenId, CancellationToken cancellationToken = default)
        {
            GoogleJsonWebSignature.ValidationSettings validationSettings = new GoogleJsonWebSignature.ValidationSettings();

            validationSettings.Audience = new List<string>() { _configuration["Authentication:Google:ClientId"] ?? string.Empty };


            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(googleTokenId, validationSettings);

                var firstName = payload.Name.Split(' ')[0];

                var lastName = payload.Name.Split(' ').Length > 1 ? payload.Name.Split(' ')[1] : string.Empty;

                var email = payload.Email;

                var randomPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

                ApplicationUser applicationUser = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    EmailConfirmed = true
                };

                var createApplicationUserResult = (await _userManager.CreateAsync(applicationUser, randomPassword)).ToResult();

                if (!createApplicationUserResult.IsSuccess)
                    return Result<IdentityUserDto>.Failure(createApplicationUserResult.Errors);

                var addToRoleResult = (await _userManager.AddToRoleAsync(applicationUser, Roles.Staff)).ToResult();

                if (!addToRoleResult.IsSuccess)
                    return Result<IdentityUserDto>.Failure(addToRoleResult.Errors);



                return Result<IdentityUserDto>.Success(MapToDto(applicationUser, new List<string> { Roles.Staff }));


            }
            catch (Exception)
            {
                return Result<IdentityUserDto>.Failure(Errors.IdentityErrors.InvalidGoogleIdToken);

            }

        }

        #endregion

    }
}