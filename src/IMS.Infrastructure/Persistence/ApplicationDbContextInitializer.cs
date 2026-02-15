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
                ("Bird Supplies", "Supplies for birds including cages and food"),
                ("Cat Beds", "Comfortable beds for cats"),
                ("Dog Toys", "Toys for dogs to play with"),
                ("Aquariums", "Tanks for keeping fish"),
                ("Pet Shampoo & Conditioner", "Grooming products for pets"),
                ("Pet Training Aids", "Tools for training pets"),
                ("Pet Waste Bags", "Bags for disposing pet waste"),
                ("Pet Doors", "Doors allowing pets to enter/exit"),
                ("Pet Carriers & Crates", "Carriers and crates for transporting pets"),
                ("Live Animals", "Live pets and animals"),
                ("Law Enforcement Cuffs", "Restraints for law enforcement"),
                ("Manufacturing Equipment", "Equipment for manufacturing processes"),
                ("Conveyors", "Systems for transporting materials"),
                ("Stethoscopes", "Medical devices for listening to body sounds"),
                ("Hospital Beds", "Beds for hospital and homecare use"),
                ("Display Mannequins", "Mannequins for retail displays"),
                ("Microscopes", "Laboratory equipment for magnification"),
                ("Safety Signs", "Signs for safety and warnings"),
                ("Hardhats", "Protective headgear for work safety"),
                ("Tattooing Inks", "Inks used in tattooing"),
                ("Digital Cameras", "Cameras for digital photography"),
                ("Binoculars", "Optics for distant viewing"),
                ("Studio Lights", "Lighting for photography studios"),
                ("Lens Filters", "Filters for camera lenses"),
                ("Surveillance Cameras", "Cameras for security monitoring"),
                ("Telescopes", "Optics for astronomical viewing"),
                ("Photographic Paper", "Paper for printing photos"),
                ("Tripods", "Stands for cameras"),
                ("Headphones", "Audio devices for personal listening"),
                ("MP3 Players", "Portable music players"),
                ("Motherboards", "Circuit boards for computers"),
                ("Mobile Phones", "Cellular telephones"),
                ("Laptops", "Portable computers"),
                ("Audio Cables", "Cables for audio connections"),
                ("Printers", "Devices for printing documents"),
                ("Televisions", "Screens for viewing video content"),
                ("Wireless Routers", "Devices for wireless networking"),
                ("Audio Converters", "Converters for audio signals"),
                ("Wine", "Alcoholic beverages from grapes"),
                ("Apples", "Fresh fruits"),
                ("Cheese", "Dairy products from milk"),
                ("Chips", "Snack foods"),
                ("Breads", "Bakery products"),
                ("Coffee", "Beverages from roasted beans"),
                ("Ketchup", "Condiments and sauces"),
                ("Rice", "Grains and cereals"),
                ("Mattresses", "Bedding for sleeping"),
                ("Kitchen Chairs", "Chairs for dining areas"),
                ("Dining Tables", "Tables for kitchens"),
                ("Outdoor Sofas", "Furniture for outdoor seating"),
                ("Kitchen Cabinets", "Storage for kitchens"),
                ("Office Desks", "Furniture for offices"),
                ("Sofas", "Seating furniture"),
                ("Coffee Tables", "Accent tables for living rooms"),
                ("Home Doors", "Doors for buildings"),
                ("Flooring", "Materials for floors"),
                ("Bathtubs", "Fixtures for bathing"),
                ("Toilets", "Plumbing fixtures for waste disposal"),
                ("Screws", "Fasteners for hardware"),
                ("Generators", "Power supply devices"),
                ("Drill Bits", "Accessories for drills"),
                ("Axes", "Tools for chopping"),
                ("Bolt Cutters", "Cutters for bolts"),
                ("Windows", "Building materials for light and air"),
                ("Plates", "Dinnerware for eating"),
                ("Mugs", "Drinkware for beverages"),
                ("Shovels", "Gardening tools"),
                ("Lamps", "Lighting fixtures"),
                ("Pillows", "Bedding accessories"),
                ("Swimming Pools", "Pools for swimming"),
                ("Forks", "Flatware for eating"),
                ("Houseplants", "Indoor plants"),
                ("Lawn Mowers", "Equipment for lawn care"),
                ("Bath Towels", "Linens for bathing"),
                ("Backpacks", "Bags for carrying items"),
                ("Duffel Bags", "Travel bags"),
                ("Suitcases", "Luggage for travel"),
                ("Garment Bags", "Bags for clothing"),
                ("Messenger Bags", "Shoulder bags"),
                ("Luggage Tags", "Tags for identifying luggage"),
                ("Diaper Bags", "Bags for baby supplies"),
                ("Train Cases", "Cases for cosmetics"),
                ("Toiletry Bags", "Bags for personal care items"),
                ("Swords", "Weapons for mature categories"),
                ("Print Books", "Physical books"),
                ("DVDs", "Video media"),
                ("Music CDs", "Audio recordings"),
                ("Magazines", "Periodical publications"),
                ("Sheet Music", "Printed music"),
                ("Camera Manuals", "Product manuals for cameras"),
                ("Binders", "Office organization tools"),
                ("Envelopes", "Paper products for mailing"),
                ("Calculators", "Office equipment for math"),
                ("Staplers", "Instruments for fastening paper"),
                ("White Boards", "Presentation supplies"),
                ("Pens", "Writing instruments"),
                ("File Folders", "Filing organization"),
                ("Sticky Notes", "General office supplies"),
                ("Bananas", "Fresh fruits"),
                ("Yogurt", "Dairy products")
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

            var productData = new List<(string name, string sku, string desc, decimal price, string manufacturer, int catIndex)>
            {
                ("Premium Bird Seed Mix", "BIRD-001", "Nutritious seed mix for birds", 12.99m, "Avian Foods", 0),
                ("Cozy Cat Bed", "CAT-001", "Soft bed for cats", 29.99m, "Pet Comfort", 1),
                ("Durable Dog Chew Toy", "DOG-001", "Tough toy for dogs", 9.99m, "Pet Play", 2),
                ("Glass Aquarium 20 Gal", "FISH-001", "20 gallon fish tank", 49.99m, "Aqua World", 3),
                ("Herbal Pet Shampoo", "GROOM-001", "Natural shampoo for pets", 15.99m, "Clean Paws", 4),
                ("Clicker Training Kit", "TRAIN-001", "Kit for pet training", 7.99m, "Smart Pet", 5),
                ("Biodegradable Waste Bags", "WASTE-001", "Eco-friendly pet waste bags", 5.99m, "Green Clean", 6),
                ("Automatic Pet Door", "DOOR-001", "Sensor-activated pet door", 89.99m, "Pet Access", 7),
                ("Portable Pet Carrier", "CARRY-001", "Carrier for small pets", 39.99m, "Travel Pet", 8),
                ("Hamster Live Animal", "ANIMAL-001", "Live hamster pet", 19.99m, "Pet Farm", 9),
                ("Police Handcuffs", "LAW-001", "Standard law enforcement cuffs", 24.99m, "Security Gear", 10),
                ("3D Printer", "MANUF-001", "Desktop 3D printer", 299.99m, "MakeTech", 11),
                ("Belt Conveyor System", "HANDLE-001", "Industrial conveyor belt", 499.99m, "Material Movers", 12),
                ("Digital Stethoscope", "MED-001", "Electronic stethoscope", 149.99m, "Health Tech", 13),
                ("Adjustable Hospital Bed", "BED-001", "Electric hospital bed", 999.99m, "Med Furnish", 14),
                ("Full Body Mannequin", "DISPLAY-001", "Retail display mannequin", 79.99m, "Store Fixtures", 15),
                ("Compound Microscope", "LAB-001", "High-power microscope", 199.99m, "Science Tools", 16),
                ("Caution Wet Floor Sign", "SIGN-001", "Safety warning sign", 14.99m, "Safety Pro", 17),
                ("Construction Hardhat", "SAFETY-001", "Protective hardhat", 19.99m, "Work Gear", 18),
                ("Black Tattoo Ink", "TATTOO-001", "Professional tattoo ink", 29.99m, "Ink Art", 19),
                ("Canon EOS Digital Camera", "CAM-001", "DSLR camera", 599.99m, "Canon", 20),
                ("Nikon Binoculars", "OPTIC-001", "High-magnification binoculars", 129.99m, "Nikon", 21),
                ("LED Studio Light Kit", "LIGHT-001", "Photography lighting set", 199.99m, "Photo Pro", 22),
                ("UV Lens Filter", "FILTER-001", "Protective lens filter", 19.99m, "Optics Inc", 23),
                ("HD Security Camera", "SURV-001", "Surveillance camera", 89.99m, "Secur Cam", 24),
                ("Celestron Telescope", "TELE-001", "Astronomy telescope", 249.99m, "Celestron", 25),
                ("Kodak Photo Paper", "PAPER-001", "Glossy photo paper", 14.99m, "Kodak", 26),
                ("Manfrotto Tripod", "TRIPOD-001", "Professional tripod", 99.99m, "Manfrotto", 27),
                ("Sony Noise-Cancelling Headphones", "AUDIO-001", "Wireless headphones", 299.99m, "Sony", 28),
                ("Apple iPod Touch", "MP3-001", "Portable media player", 199.99m, "Apple", 29),
                ("ASUS Motherboard", "CIRC-001", "Computer motherboard", 149.99m, "ASUS", 30),
                ("Samsung Galaxy Smartphone", "PHONE-001", "Android mobile phone", 799.99m, "Samsung", 31),
                ("Dell Inspiron Laptop", "COMP-001", "Portable laptop computer", 599.99m, "Dell", 32),
                ("HDMI Cable", "CABLE-001", "High-speed audio/video cable", 9.99m, "Cable Co", 33),
                ("HP LaserJet Printer", "PRINT-001", "Laser printer", 199.99m, "HP", 34),
                ("LG Smart TV", "TV-001", "4K smart television", 499.99m, "LG", 35),
                ("TP-Link Router", "NET-001", "Wireless router", 59.99m, "TP-Link", 36),
                ("DAC Audio Converter", "CONV-001", "Digital to analog converter", 99.99m, "Audio Tech", 37),
                ("Cabernet Sauvignon Wine", "BEV-001", "Red wine", 24.99m, "Vineyard Estates", 38),
                ("Red Delicious Apple", "FRUIT-001", "Fresh apple", 1.99m, "Fruit Farm", 39),
                ("Cheddar Cheese Block", "DAIRY-001", "Sharp cheddar cheese", 5.99m, "Dairy Co", 40),
                ("Potato Chips Bag", "SNACK-001", "Crispy potato chips", 3.99m, "Snack Brands", 41),
                ("Whole Wheat Bread", "BAKE-001", "Sliced bread", 2.99m, "Bakery Inc", 42),
                ("Ground Coffee Beans", "BEV-002", "Medium roast coffee", 9.99m, "Coffee Roasters", 43),
                ("Heinz Ketchup Bottle", "COND-001", "Tomato ketchup", 4.99m, "Heinz", 44),
                ("Basmati Rice Bag", "GRAIN-001", "Long grain rice", 8.99m, "Rice Mills", 45),
                ("Memory Foam Mattress", "BED-0012", "Queen size mattress", 399.99m, "Sleep Well", 46),
                ("Wooden Dining Chair", "CHAIR-001", "Kitchen chair", 79.99m, "Furnish Home", 47),
                ("Oak Dining Table", "TABLE-001", "Large dining table", 299.99m, "Table Makers", 48),
                ("Patio Sofa Set", "OUT-001", "Outdoor sofa", 499.99m, "Garden Furn", 49),
                ("Wall-Mount Kitchen Cabinet", "CAB-001", "Storage cabinet", 149.99m, "Kitchen Pros", 50),
                ("Executive Office Desk", "DESK-001", "Wooden desk", 249.99m, "Office Furn", 51),
                ("Leather Sofa", "SOFA-001", "Comfortable sofa", 699.99m, "Living Room", 52),
                ("Glass Coffee Table", "ACC-001", "Modern coffee table", 129.99m, "Decor Home", 53),
                ("Interior Wooden Door", "DOOR-002", "Home door", 199.99m, "Door Works", 54),
                ("Hardwood Flooring", "FLOOR-001", "Oak flooring", 5.99m, "Floor Inc", 55), // price per sq ft
                ("Freestanding Bathtub", "BATH-001", "Luxury bathtub", 799.99m, "Bath Pros", 56),
                ("Porcelain Toilet", "TOIL-001", "Standard toilet", 199.99m, "Plumb Co", 57),
                ("Stainless Steel Screws", "FAST-001", "Pack of screws", 4.99m, "Hardware Inc", 58),
                ("Portable Generator", "GEN-001", "Power generator", 599.99m, "Power Tech", 59),
                ("Carbide Drill Bit Set", "DRILL-001", "Drill bits", 19.99m, "Tool Masters", 60),
                ("Felling Axe", "TOOL-001", "Sharp axe", 39.99m, "Tool Co", 61),
                ("Heavy Duty Bolt Cutter", "CUT-001", "Bolt cutter", 49.99m, "Cut Tools", 62),
                ("Double Pane Window", "WIN-001", "Energy efficient window", 299.99m, "Window Works", 63),
                ("Ceramic Dinner Plate", "DINE-001", "White plate", 9.99m, "Tableware Co", 64),
                ("Ceramic Coffee Mug", "DRINK-001", "12oz mug", 7.99m, "Mug Makers", 65),
                ("Garden Shovel", "GARD-001", "Steel shovel", 19.99m, "Garden Tools", 66),
                ("Table Lamp", "LIGHT-002", "Desk lamp", 29.99m, "Light Inc", 67),
                ("Memory Foam Pillow", "BED-002", "Standard pillow", 19.99m, "Sleep Co", 68),
                ("Inflatable Swimming Pool", "POOL-001", "Backyard pool", 199.99m, "Pool Fun", 69),
                ("Stainless Steel Fork", "FLAT-001", "Eating fork", 2.99m, "Utensil Co", 70),
                ("Ficus Houseplant", "PLANT-001", "Indoor ficus tree", 29.99m, "Green House", 71),
                ("Electric Lawn Mower", "LAWN-001", "Push mower", 249.99m, "Lawn Care", 72),
                ("Cotton Bath Towel", "LINEN-001", "Large towel", 14.99m, "Linen Co", 73),
                ("Hiking Backpack", "BAG-001", "40L backpack", 59.99m, "Outdoor Gear", 74),
                ("Canvas Duffel Bag", "BAG-002", "Travel duffel", 49.99m, "Travel Bags", 75),
                ("Rolling Suitcase", "LUG-001", "Carry-on suitcase", 89.99m, "Luggage Pro", 76),
                ("Suit Garment Bag", "BAG-003", "Garment carrier", 29.99m, "Suit Protect", 77),
                ("Leather Messenger Bag", "BAG-004", "Shoulder bag", 79.99m, "Bag Co", 78),
                ("Personalized Luggage Tag", "ACC-002", "ID tag", 4.99m, "Tag Makers", 79),
                ("Baby Diaper Bag", "BAG-005", "Diaper backpack", 39.99m, "Baby Gear", 80),
                ("Makeup Train Case", "CASE-001", "Cosmetic case", 29.99m, "Beauty Box", 81),
                ("Hanging Toiletry Bag", "BAG-006", "Travel toiletry bag", 19.99m, "Travel Essentials", 82),
                ("Katana Sword", "WEAP-001", "Japanese sword", 199.99m, "Weapon Collect", 83),
                ("Harry Potter Book", "BOOK-001", "Fantasy novel", 14.99m, "Penguin Books", 84),
                ("Inception DVD", "VIDEO-001", "Movie DVD", 9.99m, "Warner Bros", 85),
                ("Taylor Swift CD", "MUSIC-001", "Music album", 12.99m, "Universal Music", 86),
                ("National Geographic Magazine", "MAG-001", "Monthly magazine", 5.99m, "Nat Geo", 87),
                ("Piano Sheet Music", "MUSIC-002", "Printed sheet music", 7.99m, "Music Pub", 88),
                ("Canon Camera Manual", "MAN-001", "User manual", 4.99m, "Canon", 89),
                ("3-Ring Binder", "OFF-001", "Office binder", 3.99m, "Office Supply", 90),
                ("Business Envelopes", "PAPER-002", "Pack of envelopes", 6.99m, "Envelope Co", 91),
                ("Scientific Calculator", "CALC-001", "Advanced calculator", 19.99m, "Texas Instruments", 92),
                ("Heavy Duty Stapler", "OFF-002", "Desk stapler", 14.99m, "Staple Pro", 93),
                ("Dry Erase Whiteboard", "PRES-001", "Wall whiteboard", 29.99m, "Board Co", 94),
                ("Ballpoint Pen Pack", "WRITE-001", "Pack of pens", 4.99m, "Pen Inc", 95),
                ("Manila File Folders", "FILE-001", "Pack of folders", 7.99m, "File Sys", 96),
                ("Colorful Sticky Notes", "NOTE-001", "Pack of sticky notes", 3.99m, "Note Co", 97),
                ("Fresh Bananas Bunch", "FRUIT-002", "Ripe bananas", 1.49m, "Fruit Farm", 98),
                ("Greek Yogurt Container", "DAIRY-002", "Plain yogurt", 2.99m, "Dairy Co", 99),
                ("Wireless Mouse", "COMP-002", "Computer mouse", 19.99m, "Logitech", 33), // additional products to reach 150
                ("Bluetooth Speaker", "AUDIO-002", "Portable speaker", 49.99m, "JBL", 28),
                ("External Hard Drive", "STORAGE-001", "1TB hard drive", 59.99m, "Seagate", 32),
                ("Gaming Keyboard", "COMP-003", "Mechanical keyboard", 79.99m, "Razer", 33),
                ("Fitness Tracker", "WEAR-001", "Activity tracker", 99.99m, "Fitbit", 13),
                ("Electric Kettle", "KITCH-001", "Kitchen appliance", 29.99m, "KitchenAid", 50),
                ("Vacuum Cleaner", "HOME-001", "Upright vacuum", 149.99m, "Dyson", 55),
                ("Blender", "KITCH-002", "High-speed blender", 69.99m, "Vitamix", 50),
                ("Air Purifier", "HOME-002", "HEPA air purifier", 129.99m, "Honeywell", 55),
                ("Smart Thermostat", "HOME-003", "WiFi thermostat", 199.99m, "Nest", 55),
                ("Espresso Machine", "KITCH-003", "Coffee maker", 299.99m, "DeLonghi", 50),
                ("Yoga Mat", "SPORT-001", "Exercise mat", 19.99m, "Gaiam", 18),
                ("Running Shoes", "FOOT-001", "Athletic shoes", 89.99m, "Nike", 18),
                ("Dumbbell Set", "FIT-001", "Weight set", 49.99m, "CAP Barbell", 18),
                ("Tennis Racket", "SPORT-002", "Sports racket", 59.99m, "Wilson", 18),
                ("Bicycle Helmet", "SAFETY-002", "Protective helmet", 39.99m, "Bell", 18),
                ("Camping Tent", "OUT-002", "4-person tent", 129.99m, "Coleman", 49),
                ("Sleeping Bag", "OUT-003", "Outdoor sleeping bag", 69.99m, "REI", 49),
                ("Hiking Boots", "FOOT-002", "Waterproof boots", 99.99m, "Merrell", 49),
                ("Portable Grill", "OUT-004", "BBQ grill", 79.99m, "Weber", 49),
                ("Fishing Rod", "SPORT-003", "Fishing pole", 49.99m, "Shimano", 18),
                ("Golf Clubs Set", "SPORT-004", "Beginner golf set", 299.99m, "Callaway", 18),
                ("Skateboard", "SPORT-005", "Street skateboard", 59.99m, "Element", 18),
                ("Ski Goggles", "SPORT-006", "Winter sports goggles", 79.99m, "Oakley", 18),
                ("Surfboard", "SPORT-007", "Beginner surfboard", 199.99m, "BIC Sport", 18),
                ("Basketball", "SPORT-008", "Regulation basketball", 29.99m, "Spalding", 18),
                ("Soccer Ball", "SPORT-009", "Size 5 soccer ball", 19.99m, "Adidas", 18),
                ("Baseball Glove", "SPORT-010", "Leather glove", 49.99m, "Rawlings", 18),
                ("Volleyball", "SPORT-011", "Beach volleyball", 24.99m, "Wilson", 18),
                ("Badminton Set", "SPORT-012", "Racket set", 39.99m, "Yonex", 18),
                ("Table Tennis Paddle", "SPORT-013", "Ping pong paddle", 14.99m, "Butterfly", 18),
                ("Jump Rope", "FIT-002", "Fitness jump rope", 9.99m, "Everlast", 18),
                ("Resistance Bands", "FIT-003", "Exercise bands", 19.99m, "TheraBand", 18),
                ("Kettlebell", "FIT-004", "20lb kettlebell", 39.99m, "CAP Barbell", 18),
                ("Treadmill", "FIT-005", "Home treadmill", 499.99m, "NordicTrack", 18),
                ("Stationary Bike", "FIT-006", "Exercise bike", 299.99m, "Schwinn", 18),
                ("Elliptical Machine", "FIT-007", "Cardio machine", 599.99m, "Sole Fitness", 18),
                ("Rowing Machine", "FIT-008", "Indoor rower", 399.99m, "Concept2", 18),
                ("Weight Bench", "FIT-009", "Adjustable bench", 149.99m, "Marcy", 18),
                ("Pull-Up Bar", "FIT-010", "Doorway bar", 29.99m, "Iron Gym", 18),
                ("Ab Roller", "FIT-011", "Core trainer", 19.99m, "Perfect Fitness", 18),
                ("Medicine Ball", "FIT-012", "10lb ball", 24.99m, "Champion Sports", 18),
                ("Foam Roller", "FIT-013", "Massage roller", 14.99m, "TriggerPoint", 18),
                ("Balance Ball", "FIT-014", "Stability ball", 19.99m, "Gaiam", 18),
                ("Battle Rope", "FIT-015", "Training rope", 49.99m, "Power Guidance", 18),
                ("Punching Bag", "FIT-016", "Heavy bag", 79.99m, "Everlast", 18),
                ("Boxing Gloves", "FIT-017", "Training gloves", 39.99m, "Venum", 18),
                ("Karate Gi", "SPORT-014", "Martial arts uniform", 49.99m, "Century Martial Arts", 18),
                ("Judo Mat", "SPORT-015", "Tatami mat", 99.99m, "Zebra Mats", 18),
                ("Fencing Sword", "SPORT-016", "Epee sword", 89.99m, "Absolute Fencing", 83), // linking to sword category
                ("Archery Bow", "SPORT-017", "Compound bow", 199.99m, "Bear Archery", 18),
                ("Darts Set", "GAME-001", "Steel tip darts", 19.99m, "Viper", 96),
                ("Chess Board", "GAME-002", "Wooden chess set", 29.99m, "House of Staunton", 96),
                ("Monopoly Board Game", "TOY-001", "Classic board game", 19.99m, "Hasbro", 96)
            };

            foreach (var data in productData)
            {
                products.Add(Product.Create(data.name, data.sku, data.desc, data.price, data.manufacturer, categories[data.catIndex].Id).Value!);
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
                            ForceSetCreatedBy(sale, users[0].Id.ToString());
                            transactions.Add(sale);
                        }
                        else
                        {
                            int qty = Random.Shared.Next(20, 50);
                            inventory.Restock(qty);
                            var purchase = Transaction.RecordPurchase(product.Id, qty, product.Price).Value!;
                            ForceSetCreatedDate(purchase, productCursorDate);
                            ForceSetCreatedBy(purchase, users[0].Id.ToString());
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

        private void ForceSetCreatedBy(Transaction trans, string CreatedBy)
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
                await _roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}