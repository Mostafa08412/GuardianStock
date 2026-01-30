using IMS.Domain.Enums;
using IMS.Domain.Users;
using IMS.Infrastructure.Persistence;
using IMS.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{

    public static class ApplicationDbContextInitializerDependencyInjection
    {

        public async static void RegisterInitializer(this IHost applicationBuilder)
        {
            using (var scope = applicationBuilder.Services.CreateScope())
            {
                var ApplicationDbContextInitializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

                await ApplicationDbContextInitializer.InitializeDatabase();
                await ApplicationDbContextInitializer.TrySeedAsync();
            }
        }


    }



    public class ApplicationDbContextInitializer
    {


        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ILogger<ApplicationDbContextInitializer> _logger;

        public ApplicationDbContextInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ApplicationDbContextInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeDatabase()
        {

            _logger.LogInformation("Initializing database...");
            try
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }


        }

        public async Task TrySeedAsync()
        {
            _logger.LogInformation("Seeding database...");
            try
            {
                await SeedDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task CreateIdentityUserAsync(
            ApplicationUser user,
            string password,
            string role)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create user {user.UserName}: {errors}");
            }

            var roleResult = await _userManager.AddToRoleAsync(user, role.ToString());

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new Exception($"Failed to assign role to {user.UserName}: {errors}");
            }
        }
        private async Task EnsureRoleAsync(string role)
        {


            if (!await _roleManager.RoleExistsAsync(role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(role));

                if (!result.Succeeded)
                    throw new Exception($"Failed to create role: {role}");
            }
        }


        private async Task SeedDataAsync()
        {
            // =========================
            // 1. Ensure Roles Exist
            // =========================
            await EnsureRoleAsync(Roles.Admin);
            await EnsureRoleAsync(Roles.Manager);
            await EnsureRoleAsync(Roles.Staff);

            // =========================
            // 2. Prevent Reseeding
            // =========================
            if (_context.Set<User>().Any())
                return;

            // =========================
            // 3. Create Identity Users
            // =========================
            var adminIdentityUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = "Administrator@localhost",
                FirstName = "System",
                LastName = "Administrator",
                EmailConfirmed = true
            };

            var managerIdentityUser = new ApplicationUser
            {
                UserName = "Manager",
                Email = "Manager@localhost",
                FirstName = "System",
                LastName = "Manager",
                EmailConfirmed = true
            };

            var staffIdentityUser = new ApplicationUser
            {
                UserName = "Staff",
                Email = "Staff@localhost",
                FirstName = "System",
                LastName = "Staff",
                EmailConfirmed = true
            };

            await CreateIdentityUserAsync(adminIdentityUser, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(managerIdentityUser, "Manager@123", Roles.Manager);
            await CreateIdentityUserAsync(staffIdentityUser, "Staff@123", Roles.Staff);

            // =========================
            // 4. Create Domain Users
            // =========================
            var domainUsers = new List<User>
    {
        User.Create(
            adminIdentityUser.Id,
            adminIdentityUser.FirstName,
            adminIdentityUser.LastName,
            adminIdentityUser.UserName!,
            adminIdentityUser.Email!
        ).Value!,
        User.Create(
            managerIdentityUser.Id,
            managerIdentityUser.FirstName,
            managerIdentityUser.LastName,
            managerIdentityUser.UserName!,
            managerIdentityUser.Email!
        ).Value!,
        User.Create(
            staffIdentityUser.Id,
            staffIdentityUser.FirstName,
            staffIdentityUser.LastName,
            staffIdentityUser.UserName!,
            staffIdentityUser.Email!
        ).Value!
    };

            // =========================
            // 7. Persist Data
            // =========================
            _context.Set<User>().AddRange(domainUsers);


            await _context.SaveChangesAsync();
        }

    }
}
