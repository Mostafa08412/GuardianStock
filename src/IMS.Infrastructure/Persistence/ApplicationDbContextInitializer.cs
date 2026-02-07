using IMS.Application.Common.Interfaces;
using IMS.Domain.Abstractions;
using IMS.Domain.Categories;
using IMS.Domain.Enums;
using IMS.Domain.Inventories;
using IMS.Domain.Products;
using IMS.Domain.StockHistories;
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

        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly IDateTime _dateTime;

        public ApplicationDbContextInitializer(IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ApplicationDbContextInitializer> logger, IDateTime dateTime)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _dateTime = dateTime;
            _unitOfWork = unitOfWork;

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
                await SeedRoles();
                await SeedApplicationUsers();

                // Domain Seeding (Refactored to async Task)
                await SeedDomainUserAsync();
                await SeedCategoriesAsync();
                await SeedProductsAsync();
                await SeedInventoriesAsync();
                //(_dateTime as SettableDateProvider).UTCNow = new DateTime(2025, 5, 1);
                // await SeedTransactions();
                // (_dateTime as SettableDateProvider).UTCNow = new DateTime(2025, 5, 1);

                await SeedDataAsync();
                //(_dateTime as SettableDateProvider).UTCNow = DateTime.UtcNow;

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
            var clock = _dateTime as SettableDateProvider;
            if (clock == null) return;
            foreach (var product in products)
            {
                var inventory = Inventory.Create(Random.Shared.Next(90, 150), Random.Shared.Next(10, 30), product.Id).Value!;

                inventories.Add(inventory);
                _context.StockHistories.Add(StockHistory.Create(inventory.Id, null, clock.UTCNow, inventory.Quantity));

            }
        }

        private async Task SeedTransactions()
        {
            if (await _context.Transactions.AnyAsync()) return;

            var clock = _dateTime as SettableDateProvider;
            if (clock == null) return;

            // بداية تاريخ العمليات
            DateTime startDate = new DateTime(2024, 8, 1, 9, 0, 0);

            foreach (var product in products)
            {
                var inventory = inventories.First(x => x.ProductId == product.Id);

                // نقطة انطلاق زمنية لكل منتج لضمان عدم تداخل التواريخ
                DateTime productCursorDate = startDate.AddMinutes(Random.Shared.Next(0, 1440));

                for (int monthOffset = 0; monthOffset < 4; monthOffset++)
                {
                    int transactionsInMonth = Random.Shared.Next(8, 15);

                    for (int i = 0; i < transactionsInMonth; i++)
                    {
                        // نباعد بين العمليات زمنياً (مثلاً كل عملية بعد 18 إلى 36 ساعة من السابقة)
                        productCursorDate = productCursorDate.AddHours(Random.Shared.Next(18, 36));
                        clock.UTCNow = productCursorDate;

                        // تحديد نوع العملية بناءً على حالة المخزون الحالية
                        bool isSale = inventory.Quantity > 10 && Random.Shared.Next(0, 100) > 40;

                        if (isSale)
                        {
                            int qty = Random.Shared.Next(1, 5);

                            // استخدام الـ Domain Method
                            inventory.Ship(qty);

                            // تسجيل العملية في الـ History بنفس التاريخ
                            var sale = Transaction.RecordSale(product.Id, qty, product.Price).Value!;

                            // نضمن أن تاريخ العملية هو نفس تاريخ الـ clock تماماً
                            ForceSetCreatedDate(sale, productCursorDate);
                            transactions.Add(sale);
                        }
                        else
                        {
                            int qty = Random.Shared.Next(20, 50);

                            // استخدام الـ Domain Method
                            inventory.Restock(qty);

                            var purchase = Transaction.RecordPurchase(product.Id, qty, product.Price).Value!;

                            ForceSetCreatedDate(purchase, productCursorDate);
                            transactions.Add(purchase);
                        }

                        _context.StockHistories.Add(StockHistory.Create(inventory.Id, null, clock.UTCNow, inventory.Quantity));
                    }
                }
            }
        }
        private void ForceSetCreatedDate(Transaction trans, DateTime date)
        {
            var prop = typeof(Transaction).GetProperty("CreatedOnUTC");
            var prop2 = typeof(Transaction).GetProperty("UpdatedOnUTC");
            if (prop != null)
            {
                prop.SetValue(trans, date);
                prop2.SetValue(trans, date);
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



            await _unitOfWork.Complete(default);


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

            var admin1 = new ApplicationUser { UserName = "Admin1", Email = "Admin1@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var admin2 = new ApplicationUser { UserName = "Admin2", Email = "Admin2@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var admin3 = new ApplicationUser { UserName = "Admin3", Email = "Admin3@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var admin4 = new ApplicationUser { UserName = "Admin4", Email = "Admin4@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var admin5 = new ApplicationUser { UserName = "Admin5", Email = "Admin5@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var admin6 = new ApplicationUser { UserName = "Admin6", Email = "Admin6@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };
            var admin7 = new ApplicationUser { UserName = "Admin7", Email = "Admin7@localhost", FirstName = "System", LastName = "Admin", EmailConfirmed = true };

            await CreateIdentityUserAsync(admin1, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(admin2, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(admin3, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(admin4, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(admin5, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(admin6, "Admin@123", Roles.Admin);
            await CreateIdentityUserAsync(admin7, "Admin@123", Roles.Admin);

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


    }
}
