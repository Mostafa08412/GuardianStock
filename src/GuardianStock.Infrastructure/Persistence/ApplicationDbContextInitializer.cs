using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Categories;
using GuardianStock.Domain.Enums;
using GuardianStock.Domain.Inventories;
using GuardianStock.Domain.Products;
using GuardianStock.Domain.StockHistories;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Users;
using GuardianStock.Infrastructure.Common;
using GuardianStock.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardianStock.Infrastructure.Persistence
{
    public static class ApplicationDbContextInitializerDependencyInjection
    {
        // Changed to async Task to avoid async void side effects
        public async static Task ResetDatabaseIfExists(this IHost applicationBuilder)
        {
            using (var scope = applicationBuilder.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
                await initializer.ClearDatabase();
            }
        }
        public async static Task InitializeDatabase(this IHost applicationBuilder)
        {
            using (var scope = applicationBuilder.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
                await initializer.InitializeDatabase();
            }
        }

        public async static Task SeedData(this IHost applicationBuilder)
        {
            using (var scope = applicationBuilder.Services.CreateScope())
            {
                var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
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
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<ApplicationDbContextInitializer> _logger;
        private readonly IDateTime _dateTime;

        public ApplicationDbContextInitializer(IUnitOfWork unitOfWork, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, ILogger<ApplicationDbContextInitializer> logger, IDateTime dateTime)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _dateTime = dateTime;
            _unitOfWork = unitOfWork;
        }
        public async Task ClearDatabase()
        {
            _logger.LogInformation("Clearing database...");
            try
            {
                await _context.Database.EnsureDeletedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while clearing the database.");
                throw;
            }
        }
        public async Task InitializeDatabase()
        {
            _logger.LogInformation("Initializing database...");
            try
            {
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
                await SeedDomainUserAsync();
                await SeedCategoriesAsync();
                await SeedProductsAsync();
                await SeedInventoriesAsync();
                (_dateTime as SettableDateProvider)!.UTCNow = new DateTime(2025, 5, 1);
                await SeedTransactions();
                (_dateTime as SettableDateProvider)!.UTCNow = DateTime.UtcNow;
                await SeedDataAsync();
                applicationUsers.Clear();
                users.Clear();
                products.Clear();
                inventories.Clear();
                transactions.Clear();
                categories.Clear();
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

            var categoryData = new List<(string name, string desc)>
            {
                ("Beauty", "Cosmetic and beauty products"),
                ("Fragrances", "Perfumes and fragrances"),
                ("Furniture", "Home and office furniture"),
                ("Groceries", "Food and household items"),
                ("Smartphones", "Mobile phones and smartphones"),
                ("Laptops", "Portable computers"),
                ("Tablets", "Tablet computers"),
                ("Audio", "Audio equipment and headphones"),
                ("TVs", "Television sets"),
                ("Wearables", "Smartwatches and fitness trackers"),
                ("Accessories", "Tech and personal accessories"),
                ("Cameras", "Digital and mirrorless cameras"),
                ("Gaming", "Gaming consoles and accessories"),
                ("Networking", "Network equipment"),
                ("Smart Home", "Smart home devices"),
                ("Kitchen Accessories", "Kitchen tools and gadgets"),
                ("Mens Shirts", "Men's clothing - shirts"),
                ("Mens Shoes", "Men's footwear"),
                ("Mens Watches", "Men's timepieces"),
                ("Mobile Accessories", "Accessories for mobile devices")
            };

            for (int i = 0; i < categoryData.Count; i++)
            {
                var data = categoryData[i];
                categories.Add(Category.Create(data.name, data.desc).Value!);
            }
        }

        private async Task SeedProductsAsync()
        {
            if (await _context.Products.AnyAsync()) return;
            // Ensure we have categories in the list to reference
            if (!categories.Any()) return;

            var productData = new List<(string name, string sku, string desc, decimal price, string manufacturer, int catIndex, string imageUrl)>
            {
                // -- USER PROVIDED DATA (1-50) --
                ("Essence Mascara Lash Princess", "BEA-001", "The Essence Mascara Lash Princess is a popular mascara known for its volumizing and lengthening effects...", 9.99m, "Essence", 0, "https://cdn.dummyjson.com/product-images/beauty/essence-mascara-lash-princess/thumbnail.webp"),
                ("Eyeshadow Palette with Mirror", "BEA-002", "The Eyeshadow Palette with Mirror offers a versatile range of eyeshadow shades...", 19.99m, "Glamour", 0, "https://cdn.dummyjson.com/product-images/beauty/eyeshadow-palette-with-mirror/thumbnail.webp"),
                ("Powder Canister", "BEA-003", "The Powder Canister is a finely milled setting powder designed to set makeup...", 14.99m, "Velvet Touch", 0, "https://cdn.dummyjson.com/product-images/beauty/powder-canister/thumbnail.webp"),
                ("Red Lipstick", "BEA-004", "The Red Lipstick is a classic and bold choice for adding a pop of color...", 12.99m, "Chic Color", 0, "https://cdn.dummyjson.com/product-images/beauty/red-lipstick/thumbnail.webp"),
                ("Red Nail Polish", "BEA-005", "The Red Nail Polish offers a rich and glossy red hue...", 8.99m, "Nail Art", 0, "https://cdn.dummyjson.com/product-images/beauty/red-nail-polish/thumbnail.webp"),
                ("Calvin Klein CK One", "FRA-001", "CK One by Calvin Klein is a classic unisex fragrance...", 49.99m, "Calvin Klein", 1, "https://cdn.dummyjson.com/product-images/fragrances/calvin-klein-ck-one/thumbnail.webp"),
                ("Chanel Coco Noir Eau De", "FRA-002", "Coco Noir by Chanel is an elegant and mysterious fragrance...", 129.99m, "Chanel", 1, "https://cdn.dummyjson.com/product-images/fragrances/chanel-coco-noir-eau-de/thumbnail.webp"),
                ("Dior J'adore", "FRA-003", "J'adore by Dior is a luxurious and floral fragrance...", 89.99m, "Dior", 1, "https://cdn.dummyjson.com/product-images/fragrances/dior-j'adore/thumbnail.webp"),
                ("Dolce Shine Eau de", "FRA-004", "Dolce Shine by Dolce & Gabbana is a vibrant and fruity fragrance...", 69.99m, "Dolce & Gabbana", 1, "https://cdn.dummyjson.com/product-images/fragrances/dolce-shine-eau-de/thumbnail.webp"),
                ("Gucci Bloom Eau de", "FRA-005", "Gucci Bloom by Gucci is a floral and captivating fragrance...", 79.99m, "Gucci", 1, "https://cdn.dummyjson.com/product-images/fragrances/gucci-bloom-eau-de/thumbnail.webp"),
                ("Annibale Colombo Bed", "FUR-001", "The Annibale Colombo Bed is a luxurious and elegant bed frame...", 1899.99m, "Annibale Colombo", 2, "https://cdn.dummyjson.com/product-images/furniture/annibale-colombo-bed/thumbnail.webp"),
                ("Annibale Colombo Sofa", "FUR-002", "The Annibale Colombo Sofa is a sophisticated and comfortable seating option...", 2499.99m, "Annibale Colombo", 2, "https://cdn.dummyjson.com/product-images/furniture/annibale-colombo-sofa/thumbnail.webp"),
                ("Bedside Table African Cherry", "FUR-003", "The Bedside Table in African Cherry is a stylish and functional addition...", 299.99m, "WoodArt", 2, "https://cdn.dummyjson.com/product-images/furniture/bedside-table-african-cherry/thumbnail.webp"),
                ("Knoll Saarinen Executive Conference Chair", "FUR-004", "The Knoll Saarinen Executive Conference Chair is a modern and ergonomic chair...", 499.99m, "Knoll", 2, "https://cdn.dummyjson.com/product-images/furniture/knoll-saarinen-executive-conference-chair/thumbnail.webp"),
                ("Wooden Bathroom Sink With Mirror", "FUR-005", "The Wooden Bathroom Sink with Mirror is a unique and stylish addition...", 799.99m, "EcoHome", 2, "https://cdn.dummyjson.com/product-images/furniture/wooden-bathroom-sink-with-mirror/thumbnail.webp"),
                ("Apple", "GRO-001", "Fresh and crisp apples, perfect for snacking...", 1.99m, "FruitFarm", 3, "https://cdn.dummyjson.com/product-images/groceries/apple/thumbnail.webp"),
                ("Beef Steak", "GRO-002", "High-quality beef steak, great for grilling...", 12.99m, "Premium Meats", 3, "https://cdn.dummyjson.com/product-images/groceries/beef-steak/thumbnail.webp"),
                ("Cat Food", "GRO-003", "Nutritious cat food formulated to meet the dietary needs...", 8.99m, "PetNutrition", 3, "https://cdn.dummyjson.com/product-images/groceries/cat-food/thumbnail.webp"),
                ("Chicken Meat", "GRO-004", "Fresh and tender chicken meat...", 9.99m, "PoultryPrime", 3, "https://cdn.dummyjson.com/product-images/groceries/chicken-meat/thumbnail.webp"),
                ("Cooking Oil", "GRO-005", "Versatile cooking oil suitable for frying...", 4.99m, "ChefSelection", 3, "https://cdn.dummyjson.com/product-images/groceries/cooking-oil/thumbnail.webp"),
                ("Cucumber", "GRO-006", "Crisp and hydrating cucumbers...", 1.49m, "GardenFresh", 3, "https://cdn.dummyjson.com/product-images/groceries/cucumber/thumbnail.webp"),
                ("Dog Food", "GRO-007", "Specially formulated dog food...", 10.99m, "K9Choice", 3, "https://cdn.dummyjson.com/product-images/groceries/dog-food/thumbnail.webp"),
                ("Eggs", "GRO-008", "Fresh eggs, a versatile ingredient...", 2.99m, "FarmDirect", 3, "https://cdn.dummyjson.com/product-images/groceries/eggs/thumbnail.webp"),
                ("Fish Steak", "GRO-009", "Quality fish steak...", 14.99m, "SeaGrocer", 3, "https://cdn.dummyjson.com/product-images/groceries/fish-steak/thumbnail.webp"),
                ("Green Bell Pepper", "GRO-010", "Fresh and vibrant green bell pepper...", 1.29m, "VeggieGarden", 3, "https://cdn.dummyjson.com/product-images/groceries/green-bell-pepper/thumbnail.webp"),
                ("Green Chili Pepper", "GRO-011", "Spicy green chili pepper...", 0.99m, "HotGarden", 3, "https://cdn.dummyjson.com/product-images/groceries/green-chili-pepper/thumbnail.webp"),
                ("Honey Jar", "GRO-012", "Pure and natural honey in a convenient jar...", 6.99m, "BeeSweet", 3, "https://cdn.dummyjson.com/product-images/groceries/honey-jar/thumbnail.webp"),
                ("Ice Cream", "GRO-013", "Creamy and delicious ice cream...", 5.49m, "DairyDelight", 3, "https://cdn.dummyjson.com/product-images/groceries/ice-cream/thumbnail.webp"),
                ("Juice", "GRO-014", "Refreshing fruit juice...", 3.99m, "FreshSqueeze", 3, "https://cdn.dummyjson.com/product-images/groceries/juice/thumbnail.webp"),
                ("Kiwi", "GRO-015", "Nutrient-rich kiwi...", 2.49m, "TropicalFruit", 3, "https://cdn.dummyjson.com/product-images/groceries/kiwi/thumbnail.webp"),
                ("iPhone 14 Pro", "PHO-001", "The iPhone 14 Pro features a stunning 6.1-inch Super Retina XDR display...", 999.99m, "Apple", 4, "https://cdn.dummyjson.com/product-images/electronics/iphone-14-pro/thumbnail.webp"),
                ("Samsung Galaxy S23 Ultra", "PHO-002", "Flagship Android smartphone with 200MP camera...", 1199.99m, "Samsung", 4, "https://cdn.dummyjson.com/product-images/electronics/samsung-galaxy-s23-ultra/thumbnail.webp"),
                ("MacBook Air M2", "LAP-001", "Ultra-thin and light laptop with M2 chip...", 1099.99m, "Apple", 5, "https://cdn.dummyjson.com/product-images/electronics/macbook-air-m2/thumbnail.webp"),
                ("Dell XPS 13", "LAP-002", "Premium ultrabook with 13.4-inch InfinityEdge display...", 1299.99m, "Dell", 5, "https://cdn.dummyjson.com/product-images/electronics/dell-xps-13/thumbnail.webp"),
                ("iPad Pro 12.9-inch", "TAB-001", "Powerful tablet with M2 chip...", 1099.99m, "Apple", 6, "https://cdn.dummyjson.com/product-images/electronics/ipad-pro-12.9/thumbnail.webp"),
                ("Sony WH-1000XM5", "AUD-001", "Industry-leading noise-canceling wireless headphones...", 349.99m, "Sony", 7, "https://cdn.dummyjson.com/product-images/electronics/sony-wh-1000xm5/thumbnail.webp"),
                ("Bose QuietComfort Earbuds II", "AUD-002", "Premium true wireless earbuds with world-class noise cancellation...", 279.99m, "Bose", 7, "https://cdn.dummyjson.com/product-images/electronics/bose-quietcomfort-earbuds-ii/thumbnail.webp"),
                ("Samsung 55-inch QLED 4K TV", "TVS-001", "Vibrant 55-inch QLED TV with Quantum HDR...", 799.99m, "Samsung", 8, "https://cdn.dummyjson.com/product-images/electronics/samsung-55-qled-4k-tv/thumbnail.webp"),
                ("Apple Watch Series 9", "WEA-001", "Advanced smartwatch with always-on Retina display...", 399.99m, "Apple", 9, "https://cdn.dummyjson.com/product-images/electronics/apple-watch-series-9/thumbnail.webp"),
                ("Google Pixel 7", "PHO-003", "Clean Android experience smartphone...", 599.99m, "Google", 4, "https://cdn.dummyjson.com/product-images/electronics/google-pixel-7/thumbnail.webp"),
                ("Logitech MX Master 3S", "ACC-001", "High-performance wireless mouse...", 99.99m, "Logitech", 10, "https://cdn.dummyjson.com/product-images/electronics/logitech-mx-master-3s/thumbnail.webp"),
                ("Anker PowerCore 26800", "ACC-002", "Portable charger with 26800mAh capacity...", 59.99m, "Anker", 10, "https://cdn.dummyjson.com/product-images/electronics/anker-powercore-26800/thumbnail.webp"),
                ("JBL Flip 6", "AUD-003", "Portable waterproof Bluetooth speaker...", 129.99m, "JBL", 7, "https://cdn.dummyjson.com/product-images/electronics/jbl-flip-6/thumbnail.webp"),
                ("Nikon Z6 II Mirrorless Camera", "CAM-001", "Full-frame mirrorless camera with 24.5MP sensor...", 1999.99m, "Nikon", 11, "https://cdn.dummyjson.com/product-images/electronics/nikon-z6-ii/thumbnail.webp"),
                ("PlayStation 5 Console", "GAM-001", "Next-gen gaming console with ultra-high speed SSD...", 499.99m, "Sony", 12, "https://cdn.dummyjson.com/product-images/electronics/playstation-5/thumbnail.webp"),
                ("Samsung Galaxy Watch 6", "WEA-002", "Stylish smartwatch with advanced health tracking...", 299.99m, "Samsung", 9, "https://cdn.dummyjson.com/product-images/electronics/samsung-galaxy-watch-6/thumbnail.webp"),
                ("Amazon Echo Dot (5th Gen)", "SMH-001", "Smart speaker with Alexa for music...", 49.99m, "Amazon", 14, "https://cdn.dummyjson.com/product-images/electronics/amazon-echo-dot-5th-gen/thumbnail.webp"),
                ("TP-Link Archer AX55", "NET-001", "Wi-Fi 6 router with dual-band speeds up to 3Gbps...", 89.99m, "TP-Link", 13, "https://cdn.dummyjson.com/product-images/electronics/tp-link-archer-ax55/thumbnail.webp"),
                ("Canon EOS R10", "CAM-002", "Compact mirrorless camera with 24.2MP APS-C sensor...", 979.99m, "Canon", 11, "https://cdn.dummyjson.com/product-images/electronics/canon-eos-r10/thumbnail.webp"),
                ("Belkin BoostCharge Pro", "ACC-003", "3-in-1 wireless charging stand...", 139.99m, "Belkin", 10, "https://cdn.dummyjson.com/product-images/electronics/belkin-boostcharge-pro/thumbnail.webp"),

                // -- DUMMYJSON DATA (51-100) --
                ("Boxed Blender", "KIT-051", "Powerful and compact blender perfect for smoothies, shakes, and more.", 39.99m, "KitchenAid", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/boxed-blender/thumbnail.webp"),
                ("Carbon Steel Wok", "KIT-052", "Versatile cooking pan suitable for stir-frying, sautéing, and deep frying.", 29.99m, "WokLife", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/carbon-steel-wok/thumbnail.webp"),
                ("Chopping Board", "KIT-053", "Essential kitchen accessory for food preparation made from durable material.", 12.99m, "ChefHouse", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/chopping-board/thumbnail.webp"),
                ("Citrus Squeezer Yellow", "KIT-054", "Handy tool for extracting juice from citrus fruits in a vibrant yellow.", 8.99m, "ZestMaker", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/citrus-squeezer-yellow/thumbnail.webp"),
                ("Egg Slicer", "KIT-055", "Convenient tool for slicing boiled eggs evenly, perfect for salads.", 6.99m, "PrepTools", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/egg-slicer/thumbnail.webp"),
                ("Electric Stove", "KIT-056", "Portable and efficient cooking solution for small kitchens.", 49.99m, "PowerCook", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/electric-stove/thumbnail.webp"),
                ("Fine Mesh Strainer", "KIT-057", "Versatile tool for straining liquids and sifting dry ingredients.", 9.99m, "SiftPro", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/fine-mesh-strainer/thumbnail.webp"),
                ("Fork", "KIT-058", "Classic utensil for various dining and serving purposes.", 3.99m, "DineWell", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/fork/thumbnail.webp"),
                ("Glass", "KIT-059", "Versatile and elegant drinking vessel suitable for a variety of beverages.", 4.99m, "CrystalClear", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/glass/thumbnail.webp"),
                ("Grater Black", "KIT-060", "Handy kitchen tool for grating cheese, vegetables, and more.", 10.99m, "GrateIt", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/grater-black/thumbnail.webp"),
                ("Hand Blender", "KIT-061", "Versatile kitchen appliance for blending, pureeing, and mixing.", 34.99m, "MixMaster", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/hand-blender/thumbnail.webp"),
                ("Ice Cube Tray", "KIT-062", "Practical accessory for making ice cubes in various shapes.", 5.99m, "CoolTouch", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/ice-cube-tray/thumbnail.webp"),
                ("Kitchen Sieve", "KIT-063", "Versatile tool for sifting and straining dry and wet ingredients.", 7.99m, "KitchFilter", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/kitchen-sieve/thumbnail.webp"),
                ("Knife", "KIT-064", "Essential kitchen tool for chopping, slicing, and dicing.", 14.99m, "SharpEdge", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/knife/thumbnail.webp"),
                ("Lunch Box", "KIT-065", "Convenient and portable container for packing and carrying your meals.", 12.99m, "BentoPlus", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/lunch-box/thumbnail.webp"),
                ("Microwave Oven", "KIT-066", "Versatile kitchen appliance for quick and efficient cooking.", 89.99m, "WaveTech", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/microwave-oven/thumbnail.webp"),
                ("Mug Tree Stand", "KIT-067", "Stylish and space-saving solution for organizing your mugs.", 15.99m, "OrganizeIt", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/mug-tree-stand/thumbnail.webp"),
                ("Pan", "KIT-068", "Versatile cookware item for frying, sautéing, and cooking dishes.", 24.99m, "CookPro", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/pan/thumbnail.webp"),
                ("Plate", "KIT-069", "Classic and essential dishware item for serving meals.", 3.99m, "DineWell", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/plate/thumbnail.webp"),
                ("Red Tongs", "KIT-070", "Versatile kitchen tongs suitable for various cooking tasks.", 6.99m, "GripIt", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/red-tongs/thumbnail.webp"),
                ("Silver Pot With Glass Cap", "KIT-071", "Stylish and functional cookware item for boiling and simmering.", 39.99m, "PrimeCook", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/silver-pot-with-glass-cap/thumbnail.webp"),
                ("Slotted Turner", "KIT-072", "Kitchen utensil designed for flipping and turning food items.", 8.99m, "FlipMaster", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/slotted-turner/thumbnail.webp"),
                ("Spice Rack", "KIT-073", "Convenient organizer for your spices and seasonings.", 19.99m, "SpicePro", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/spice-rack/thumbnail.webp"),
                ("Spoon", "KIT-074", "Versatile kitchen utensil for stirring, serving, and tasting.", 4.99m, "StirWell", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/spoon/thumbnail.webp"),
                ("Tray", "KIT-075", "Functional and decorative item for serving snacks or drinks.", 16.99m, "ServeRight", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/tray/thumbnail.webp"),
                ("Wooden Rolling Pin", "KIT-076", "Classic kitchen tool for rolling out dough for baking.", 11.99m, "BakeArt", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/wooden-rolling-pin/thumbnail.webp"),
                ("Yellow Peeler", "KIT-077", "Handy tool for peeling fruits and vegetables with ease.", 5.99m, "PeelPro", 15, "https://cdn.dummyjson.com/product-images/kitchen-accessories/yellow-peeler/thumbnail.webp"),
                ("MacBook Pro 14 Inch M1", "LAP-078", "Powerful Apple laptop featuring the M1 Pro chip and Retina display.", 1999.99m, "Apple", 5, "https://cdn.dummyjson.com/product-images/laptops/apple-macbook-pro-14-inch-space-grey/thumbnail.webp"),
                ("Asus Zenbook Pro Dual", "LAP-079", "High-performance device with dual screens for creative professionals.", 1799.99m, "Asus", 5, "https://cdn.dummyjson.com/product-images/laptops/asus-zenbook-pro-dual-screen-laptop/thumbnail.webp"),
                ("Huawei Matebook X Pro", "LAP-080", "Slim and stylish laptop with a high-resolution touchscreen display.", 1399.99m, "Huawei", 5, "https://cdn.dummyjson.com/product-images/laptops/huawei-matebook-x-pro/thumbnail.webp"),
                ("Lenovo Yoga 920", "LAP-081", "2-in-1 convertible laptop with a flexible hinge and portability.", 1099.99m, "Lenovo", 5, "https://cdn.dummyjson.com/product-images/laptops/lenovo-yoga-920/thumbnail.webp"),
                ("DELL XPS 13 9300", "LAP-082", "Compact device featuring a borderless InfinityEdge display.", 1499.99m, "Dell", 5, "https://cdn.dummyjson.com/product-images/laptops/new-dell-xps-13-9300-laptop/thumbnail.webp"),
                ("Blue & Black Check Shirt", "MEN-083", "Stylish men's shirt featuring a classic check pattern.", 29.99m, "Fashion Trends", 16, "https://cdn.dummyjson.com/product-images/mens-shirts/blue-&-black-check-shirt/thumbnail.webp"),
                ("Gigabyte Aorus Tshirt", "MEN-084", "Cool and casual shirt for gaming enthusiasts with Aorus logo.", 24.99m, "Gigabyte", 16, "https://cdn.dummyjson.com/product-images/mens-shirts/gigabyte-aorus-men-tshirt/thumbnail.webp"),
                ("Man Plaid Shirt", "MEN-085", "Timeless and versatile men's shirt with a classic plaid pattern.", 34.99m, "Classic Wear", 16, "https://cdn.dummyjson.com/product-images/mens-shirts/man-plaid-shirt/thumbnail.webp"),
                ("Man Short Sleeve Shirt", "MEN-086", "Breezy and stylish option for warm days with a polished look.", 19.99m, "Casual Comfort", 16, "https://cdn.dummyjson.com/product-images/mens-shirts/man-short-sleeve-shirt/thumbnail.webp"),
                ("Men Check Shirt", "MEN-087", "Classic and versatile shirt suitable for various occasions.", 27.99m, "Urban Chic", 16, "https://cdn.dummyjson.com/product-images/mens-shirts/men-check-shirt/thumbnail.webp"),
                ("Nike Air Jordan 1 Red", "SHOE-088", "Iconic basketball sneaker known for style and performance.", 149.99m, "Nike", 17, "https://cdn.dummyjson.com/product-images/mens-shoes/nike-air-jordan-1-red-and-black/thumbnail.webp"),
                ("Nike Baseball Cleats", "SHOE-089", "Designed for maximum traction and performance on the field.", 79.99m, "Nike", 17, "https://cdn.dummyjson.com/product-images/mens-shoes/nike-baseball-cleats/thumbnail.webp"),
                ("Puma Future Rider", "SHOE-090", "Retrospective style blended with modern comfort for daily use.", 89.99m, "Puma", 17, "https://cdn.dummyjson.com/product-images/mens-shoes/puma-future-rider-trainers/thumbnail.webp"),
                ("Sports Sneakers White Red", "SHOE-091", "Fashionable choice for sports enthusiasts with bold colors.", 119.99m, "Off White", 17, "https://cdn.dummyjson.com/product-images/mens-shoes/sports-sneakers-off-white-&-red/thumbnail.webp"),
                ("Sports Sneakers Off White", "SHOE-092", "Unique design offering style and comfort for casual occasions.", 109.99m, "Off White", 17, "https://cdn.dummyjson.com/product-images/mens-shoes/sports-sneakers-off-white-red/thumbnail.webp"),
                ("Brown Leather Belt Watch", "WATCH-093", "Stylish timepiece with a classic genuine leather strap.", 89.99m, "Fashion Time", 18, "https://cdn.dummyjson.com/product-images/mens-watches/brown-leather-belt-watch/thumbnail.webp"),
                ("Longines Master Collection", "WATCH-094", "Elegant watch known for precision and high craftsmanship.", 1499.99m, "Longines", 18, "https://cdn.dummyjson.com/product-images/mens-watches/longines-master-collection/thumbnail.webp"),
                ("Rolex Cellini Date", "WATCH-095", "Classic prestigious watch with a black dial and date.", 8999.99m, "Rolex", 18, "https://cdn.dummyjson.com/product-images/mens-watches/rolex-cellini-date-black-dial/thumbnail.webp"),
                ("Rolex Cellini Moonphase", "WATCH-096", "Horological masterpiece featuring a moon phase complication.", 12999.99m, "Rolex", 18, "https://cdn.dummyjson.com/product-images/mens-watches/rolex-cellini-moonphase/thumbnail.webp"),
                ("Rolex Datejust Blue", "WATCH-097", "Iconic and versatile timepiece with a blue dial and date.", 10999.99m, "Rolex", 18, "https://cdn.dummyjson.com/product-images/mens-watches/rolex-datejust/thumbnail.webp"),
                ("Rolex Submariner", "WATCH-098", "Legendary dive watch symbol of adventure and exploration.", 13999.99m, "Rolex", 18, "https://cdn.dummyjson.com/product-images/mens-watches/rolex-submariner-watch/thumbnail.webp"),
                ("Amazon Echo Plus", "MOB-099", "Smart speaker with built-in Alexa and premium sound hub.", 99.99m, "Amazon", 19, "https://cdn.dummyjson.com/product-images/mobile-accessories/amazon-echo-plus/thumbnail.webp"),
                ("Apple Airpods", "MOB-100", "Seamless wireless audio experience with Siri integration.", 129.99m, "Apple", 19, "https://cdn.dummyjson.com/product-images/mobile-accessories/apple-airpods/thumbnail.webp")
            };

            foreach (var data in productData)
            {
                products.Add(Product.Create(data.name, data.sku, data.desc, data.price, data.manufacturer, categories[data.catIndex].Id, data.imageUrl).Value!);
            }
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
            // Start date for operations
            DateTime startDate = new DateTime(2024, 1, 1, 9, 0, 0);
            int totalMonths = (2026 - 2024) * 12 + 1; // 28 months from Jan 2024 to Apr 2026

            foreach (var product in products)
            {
                var inventory = inventories.First(x => x.ProductId == product.Id);
                // Cursor date per product to ensure progression
                DateTime productCursorDate = startDate.AddMinutes(Random.Shared.Next(0, 1440));

                for (int monthOffset = 0; monthOffset < totalMonths; monthOffset++)
                {
                    int year = 2024 + monthOffset / 12;
                    int month = 1 + (monthOffset % 12);
                    int transactionsInMonth = Random.Shared.Next(2, 5); // Reduced to avoid excessive data

                    for (int i = 0; i < transactionsInMonth; i++)
                    {
                        // Random day in the month
                        int maxDay = DateTime.DaysInMonth(year, month);
                        int day = Random.Shared.Next(1, maxDay + 1);
                        productCursorDate = new DateTime(year, month, day).AddHours(Random.Shared.Next(0, 24)).AddMinutes(Random.Shared.Next(0, 60));

                        clock.UTCNow = productCursorDate;

                        // Determine type based on current stock, ensure not below threshold
                        bool isSale = inventory.Quantity > inventory.LowStockThreshold && Random.Shared.Next(0, 100) > 40;
                        if (isSale)
                        {
                            int maxQty = inventory.Quantity - inventory.LowStockThreshold;
                            int qty = Random.Shared.Next(1, Math.Min(5, maxQty));
                            inventory.Ship(qty);
                            var sale = Transaction.RecordSale(product.Id, qty, product.Price).Value!;
                            ForceSetCreatedDate(sale, productCursorDate);
                            ForceSetCreatedBy(sale, users[0].Id);
                            transactions.Add(sale);
                        }
                        else
                        {
                            int qty = Random.Shared.Next(20, 50);
                            inventory.Restock(qty);
                            var purchase = Transaction.RecordPurchase(product.Id, qty, product.Price).Value!;
                            ForceSetCreatedDate(purchase, productCursorDate);
                            ForceSetCreatedBy(purchase, users[0].Id);
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

        private void ForceSetCreatedBy(Transaction trans, Guid CreatedBy)
        {
            var prop = typeof(Transaction).GetProperty("CreatedBy");
            if (prop != null)
            {
                prop.SetValue(trans, CreatedBy);
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

        // --- Identity Methods ---
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
                await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }
    }
}