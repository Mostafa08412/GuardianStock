using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Enums;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.Transactions;
using IMS.Domain.Users;
using IMS.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IMS.Infrastructure.Persistence
{

    public static class ApplicationDbContextInitializerDependencyInjection
    {
        // Changed to async Task to avoid async void side effects
        public async static Task RegisterInitializer(this IHost applicationBuilder)
        {
            using (var scope = applicationBuilder.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

                await initializer.InitializeDatabase();
                await initializer.TrySeedAsync();
            }
        }
    }

    public class ApplicationDbContextInitializer
    {
        // Private lists to hold data before bulk adding
        private List<ApplicationUser> applicationUsers = new();
        private List<User> users = new();
        private List<Product> products = new();
        private List<Category> categories = new();
        private List<Inventory> inventories = new();
        private List<Transaction> transactions = new();

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly IDateTime _dateTime;

        public ApplicationDbContextInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ApplicationDbContextInitializer> logger, IDateTime dateTime)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _dateTime = dateTime;
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
                // Identity Seeding (Kept as per your original structure)
                await SeedRoles();
                await SeedApplicationUsers();

                // Domain Seeding (Refactored to async Task)
                await SeedDomainUserAsync();
                await SeedCategoriesAsync();
                await SeedProductsAsync();
                await SeedInventoriesAsync();
                await SeedTransactions();

                await SeedDataAsync();

                applicationUsers = null;
                users = null;
                products = null;
                inventories = null;
                transactions = null;
                categories = null;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task SeedDomainUserAsync()
        {
            if (await _context.BusinessUsers.AnyAsync()) return;

            foreach (var appuser in applicationUsers)
            {
                var userResult = User.Create(
                    appuser.Id,
                    appuser.FirstName,
                    appuser.LastName,
                    appuser.UserName!,
                    appuser.Email!);

                if (userResult.IsSuccess)
                {
                    users.Add(userResult.Value!);
                }
            }
        }

        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync()) return;

            categories.Add(Category.Create("Electronics", "Electronic Devices").Value!);
            categories.Add(Category.Create("Furniture", "Office and Home Furniture").Value!);
            categories.Add(Category.Create("Stationery", "Office Supplies").Value!);
        }

        private async Task SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync()) return;

            // Ensure we have categories in the list to reference
            if (!categories.Any()) return;

            for (int i = 0; i < 100; i++)
            {
                products.Add(Product.Create($"Product {i + 1}", $"TECH-LAP-{i + 1}", "High-performance laptop", Random.Shared.Next(1200, 40000), "TechGiants Inc.", categories[0].Id).Value!);
                products.Add(Product.Create($"Iphone {i + 1}", $"TECH-APPL-{i + 1}", "High-performance IPhone", Random.Shared.Next(1200, 40000), "Apple Inc.", categories[1].Id).Value!);
                products.Add(Product.Create($"Samsung Odyessy G{i + 1}", $"TECH-SAM-{i + 1}", "High-performance Samsung Monitor", Random.Shared.Next(1200, 60000), "Samsung Inc.", categories[2].Id).Value!);

            }

            products.Add(Product.Create("Laptop Pro X", "TECH-LAP-001", "High-performance laptop", 1500.00m, "TechGiants Inc.", categories[0].Id).Value!);
            products.Add(Product.Create("Ergonomic Chair", "FURN-CHR-001", "Comfortable office chair", 250.00m, "ComfortSeating", categories[1].Id).Value!);
            products.Add(Product.Create("Ballpoint Pens", "STAT-PEN-001", "Box of 12", 5.99m, "StationerySupplies", categories[2].Id).Value!);
        }

        private async Task SeedInventoriesAsync()
        {
            if (await _context.Inventories.AnyAsync()) return;

            foreach (var product in products)
            {
                var inventory = Inventory.Create(Random.Shared.Next(90, 150), Random.Shared.Next(10, 30), product.Id).Value!;
                inventories.Add(inventory);
            }
        }

        private async Task SeedTransactions()
        {
            if (await _context.Transactions.AnyAsync()) return;

            DateTime currentTime = new DateTime(2025, 11, 1);

            foreach (var product in products)
            {

                var productStock = inventories.First(x => x.ProductId == product.Id).Quantity;
                var randomQuantity = Random.Shared.Next(1, productStock);

                int dayGapForSale = Random.Shared.Next(5, 64);
                var DateForSale = currentTime
                                          .AddDays(dayGapForSale)
                                          .AddHours(Random.Shared.Next(0, 24))
                                          .AddMinutes(Random.Shared.Next(0, 60))
                                          .AddMicroseconds(124 * 2300);


                int dayGapForPurchase = Random.Shared.Next(3, 64);


                var DateForPurchase = currentTime.AddDays(dayGapForPurchase)
                                      .AddHours(Random.Shared.Next(0, 24))
                                      .AddMinutes(Random.Shared.Next(0, 60))
                                      .AddMicroseconds(1234 * 2000);



                var saleTransaction = new Transaction(product.Id, randomQuantity, product.Price, DateForSale, 0);
                var purchaseTransaction = new Transaction(product.Id, randomQuantity, product.Price, DateForPurchase, 1);
                transactions.Add(saleTransaction);
                transactions.Add(purchaseTransaction);


                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 1).AddMilliseconds(841 * 1), 1));
                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 5).AddMilliseconds(841 * 2), 1));
                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 12).AddMilliseconds(841 * 3), 1));
                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 14).AddMilliseconds(841 * 74), 1));
                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 21).AddMilliseconds(841 * 5), 1));
                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 25).AddMilliseconds(841 * 6), 1));
                transactions.Add(new Transaction(product.Id, randomQuantity, product.Price, new DateTime(2026, 1, 26).AddMilliseconds(841 * 8), 1));
            }



        }

        private async Task SeedDataAsync()
        {
            // Add all collected lists to the context
            if (users.Any()) _context.BusinessUsers.AddRange(users);
            if (categories.Any()) _context.Categories.AddRange(categories);
            if (products.Any()) _context.Products.AddRange(products);
            if (inventories.Any()) _context.Inventories.AddRange(inventories);
            if (transactions.Any()) _context.Transactions.AddRange(transactions);

            AuditAddedAndModfiedEntriesInSeedMode();

            await _context.SaveChangesAsync();
        }

        // --- Identity Methods  ---

        private async Task SeedRoles()
        {
            await EnsureRoleAsync(Roles.Admin);
            await EnsureRoleAsync(Roles.Manager);
            await EnsureRoleAsync(Roles.Staff);
        }

        private async Task SeedApplicationUsers()
        {
            if (await _userManager.Users.AnyAsync()) return;

            var admin = new ApplicationUser { UserName = "Admin", Email = "Admin@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var manager = new ApplicationUser { UserName = "Manager", Email = "Manager@localhost", FirstName = "System", LastName = "Manager", EmailConfirmed = true };

            applicationUsers.AddRange(new[] { admin, manager });

            await CreateIdentityUserAsync(admin, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(manager, "Manager@123", Roles.Manager);
        }

        private async Task CreateIdentityUserAsync(ApplicationUser user, string password, string role)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded) await _userManager.AddToRoleAsync(user, role);
        }

        private async Task EnsureRoleAsync(string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        private void AuditAddedAndModfiedEntriesInSeedMode()
        {
            var seedDateTime = _dateTime.UTCNow;
            var seedCreatedOrUpdatedBy = users[0].Id;

            foreach (var entry in _context.ChangeTracker.Entries<IAuditable>())
            {
                entry.Property(X => X.UpdatedBy).CurrentValue =
                    entry.Entity.UpdatedBy == default ? seedCreatedOrUpdatedBy : entry.Entity.UpdatedBy;

                entry.Property(X => X.UpdatedOnUTC).CurrentValue = seedDateTime;


                if (entry.State == EntityState.Added)
                {
                    entry.Property(X => X.CreatedBy).CurrentValue =
                        entry.Entity.CreatedBy == default ? seedCreatedOrUpdatedBy : entry.Entity.CreatedBy;

                    entry.Property(X => X.CreatedOnUTC).CurrentValue =
                        entry.Entity.CreatedOnUTC == default ? seedDateTime : entry.Entity.CreatedOnUTC;
                }
            }
        }
    }
}
