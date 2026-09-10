using System;
using System.Collections.Generic;
using System.Linq;
using Ecommerce.Models;

namespace Ecommerce.Data
{
    public static class ProductSeeder
    {
        public static void SeedProducts(ApplicationDbContext context)
        {
            if (context.Products.Any())
            {
                return; // Database already contains product data
            }

            // 1. Ensure Categories Exist
            var categories = new List<Category>
            {
                new Category { CategoryName = "Laptops", Description = "High performance workstation and gaming laptops" },
                new Category { CategoryName = "Mobiles", Description = "5G Flagship smartphones and tablets" },
                new Category { CategoryName = "Electronics", Description = "Audio, TVs, gaming consoles and home appliances" },
                new Category { CategoryName = "Accessories", Description = "Computer peripherals, smartwatches and storage" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            var laptopCat = categories.First(c => c.CategoryName == "Laptops").CategoryId;
            var mobileCat = categories.First(c => c.CategoryName == "Mobiles").CategoryId;
            var electronicsCat = categories.First(c => c.CategoryName == "Electronics").CategoryId;
            var accessoriesCat = categories.First(c => c.CategoryName == "Accessories").CategoryId;

            // 2. Ensure Default Seller Exists
            var seller = context.Sellers.FirstOrDefault();
            if (seller == null)
            {
                var sellerUser = new User
                {
                    Username = "WebKadaiMerchant",
                    Email = "merchant@webkadai.com",
                    PasswordHash = Ecommerce.Services.PasswordHasher.Hash("Seller@123!"),
                    RoleId = 2,
                    AccountStatus = "Active",
                    CreatedDate = DateTime.Now
                };
                context.Users.Add(sellerUser);
                context.SaveChanges();

                seller = new Seller
                {
                    UserId = sellerUser.UserId,
                    BusinessName = "Web Kadai Direct Authorized Merchant",
                    Gstnumber = "29AAAAA0000A1Z5",
                    ApprovalStatus = "Approved",
                    Status = "Active"
                };
                context.Sellers.Add(seller);
                context.SaveChanges();
            }

            // 3. Seed 20 High Quality Products
            var productsSeed = new List<(string Name, string Brand, string Desc, decimal Price, int Stock, string Warranty, int CatId, string ImageUrl)>
            {
                ("Apple MacBook Pro 16\" M3 Max", "Apple", "Liquid Retina XDR display, 36GB unified memory, 1TB SSD storage with extreme performance.", 249999m, 14, "1 Year AppleCare+", laptopCat, "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=600&auto=format&fit=crop&q=80"),
                ("Sony WH-1000XM5 Wireless Headphones", "Sony", "Industry-leading noise cancellation, crystal clear calling, and 30-hour battery life.", 29990m, 25, "1 Year Warranty", electronicsCat, "https://images.unsplash.com/photo-1505740420928-5e560c06d30e?w=600&auto=format&fit=crop&q=80"),
                ("Samsung Galaxy S24 Ultra 5G", "Samsung", "Galaxy AI features, 200MP camera, built-in S-Pen, and Snapdragon 8 Gen 3 processor.", 129999m, 18, "1 Year Warranty", mobileCat, "https://images.unsplash.com/photo-1610945265064-0e34e5519bbf?w=600&auto=format&fit=crop&q=80"),
                ("Dell XPS 15 OLED Touch", "Dell", "13th Gen Intel Core i9, NVIDIA RTX 4070, 3.5K OLED InfinityEdge touch screen.", 189990m, 8, "2 Years On-Site Support", laptopCat, "https://images.unsplash.com/photo-1593642632823-8f785ba67e45?w=600&auto=format&fit=crop&q=80"),
                ("Apple Watch Ultra 2 GPS + Cellular", "Apple", "Rugged 49mm titanium case, precision dual-frequency GPS, up to 72 hours Low Power Mode.", 89900m, 12, "1 Year Warranty", accessoriesCat, "https://images.unsplash.com/photo-1546868871-7041f2a55e12?w=600&auto=format&fit=crop&q=80"),
                ("Logitech MX Master 3S Wireless Mouse", "Logitech", "Quiet clicks, 8K DPI any-surface tracking, MagSpeed electromagnetic scrolling.", 8995m, 40, "2 Years Warranty", accessoriesCat, "https://images.unsplash.com/photo-1615663245857-ac93bb7c39e7?w=600&auto=format&fit=crop&q=80"),
                ("Asus ROG Zephyrus G16 Gaming Laptop", "Asus", "Intel Core Ultra 9 processor, NVIDIA RTX 4080, OLED 240Hz Nebula Display.", 219990m, 6, "2 Years Warranty", laptopCat, "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=600&auto=format&fit=crop&q=80"),
                ("iPhone 15 Pro Max 256GB Titanium", "Apple", "A17 Pro chip with 6-core GPU, 5x Telephoto camera, Action button, USB-C 3 speed.", 149900m, 15, "1 Year AppleCare+", mobileCat, "https://images.unsplash.com/photo-1695048133142-1a20484d2569?w=600&auto=format&fit=crop&q=80"),
                ("Bose QuietComfort Ultra Earbuds", "Bose", "Spatial audio immersive listening, CustomTune technology, 6-hour playback + wireless case.", 25900m, 30, "1 Year Bose Warranty", electronicsCat, "https://images.unsplash.com/photo-1590658268037-6bf12165a8df?w=600&auto=format&fit=crop&q=80"),
                ("LG C3 55-inch OLED 4K Smart TV", "LG", "α9 AI Processor Gen6, Brightness Booster, Dolby Vision & Atmos, 120Hz gaming support.", 139990m, 5, "3 Years LG Panel Warranty", electronicsCat, "https://images.unsplash.com/photo-1593784991095-a205069470b6?w=600&auto=format&fit=crop&q=80"),
                ("Sony PlayStation 5 Slim Console", "Sony", "1TB SSD ultra-high speed storage, DualSense wireless controller, 4K 120Hz gaming output.", 54990m, 20, "1 Year Sony Warranty", electronicsCat, "https://images.unsplash.com/photo-1606813907291-d86efa9b94db?w=600&auto=format&fit=crop&q=80"),
                ("Canon EOS R6 Mark II Camera", "Canon", "24.2 MP CMOS sensor, 40 fps continuous shooting, 4K 60p uncropped video recording.", 215995m, 4, "2 Years Canon Warranty", electronicsCat, "https://images.unsplash.com/photo-1516035069371-29a1b244cc32?w=600&auto=format&fit=crop&q=80"),
                ("Samsung Odyssey OLED G9 49\" Monitor", "Samsung", "Dual QHD 240Hz 0.03ms GTG gaming curved display with Neo Quantum Processor Pro.", 129990m, 7, "3 Years Support", accessoriesCat, "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf?w=600&auto=format&fit=crop&q=80"),
                ("Keychron Q1 Pro Mechanical Keyboard", "Keychron", "75% layout QMK/VIA wireless custom mechanical keyboard with CNC aluminum body.", 16990m, 22, "1 Year Warranty", accessoriesCat, "https://images.unsplash.com/photo-1587829741301-dc798b83add3?w=600&auto=format&fit=crop&q=80"),
                ("Samsung 990 PRO 2TB NVMe PCIe 4.0 SSD", "Samsung", "Sequential read speeds up to 7,450 MB/s, optimal power efficiency and heatsink control.", 18999m, 35, "5 Years Limited Warranty", accessoriesCat, "https://images.unsplash.com/photo-1597872200969-2b65d56bd16b?w=600&auto=format&fit=crop&q=80"),
                ("iPad Pro 12.9-inch M2 Chip 256GB", "Apple", "Liquid Retina XDR display, Apple Pencil hover feature, Wi-Fi 6E ultra-fast connectivity.", 112900m, 10, "1 Year Apple Warranty", mobileCat, "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=600&auto=format&fit=crop&q=80"),
                ("Marshall Stanmore III Bluetooth Speaker", "Marshall", "Wider soundstage stereo audio, iconic vintage design, Dynamic Loudness technology.", 31999m, 16, "1 Year Warranty", electronicsCat, "https://images.unsplash.com/photo-1545454675-3531b543be5d?w=600&auto=format&fit=crop&q=80"),
                ("Dyson V15 Detect Cordless Vacuum", "Dyson", "Laser reveals microscopic dust, piezo sensor counts dust particles, up to 60 min run time.", 65900m, 9, "2 Years Dyson Warranty", electronicsCat, "https://images.unsplash.com/photo-1558317374-067fb5f30001?w=600&auto=format&fit=crop&q=80"),
                ("Nespresso Vertuo Pop Coffee Machine", "Nespresso", "Centrifusion extraction technology, 4 cup sizes, fast 30-second heat-up system.", 16500m, 28, "2 Years Nespresso Warranty", electronicsCat, "https://images.unsplash.com/photo-1517668808822-9e428824603b?w=600&auto=format&fit=crop&q=80"),
                ("Garmin Fenix 7X Pro Solar Smartwatch", "Garmin", "Power Sapphire solar charging lens, built-in LED flashlight, advanced training metrics.", 98990m, 11, "2 Years Garmin Warranty", accessoriesCat, "https://images.unsplash.com/photo-1508685096489-7aacd43bd3b1?w=600&auto=format&fit=crop&q=80")
            };

            foreach (var item in productsSeed)
            {
                var prod = new Product
                {
                    ProductName = item.Name,
                    Brand = item.Brand,
                    Description = item.Desc,
                    Warranty = item.Warranty,
                    CategoryId = item.CatId
                };
                context.Products.Add(prod);
                context.SaveChanges();

                var img = new Productimage
                {
                    ProductId = prod.ProductId,
                    ImageUrl = item.ImageUrl
                };
                context.Productimages.Add(img);

                var sellerProd = new Sellerproduct
                {
                    ProductId = prod.ProductId,
                    SellerId = seller.SellerId,
                    Price = item.Price,
                    StockQuantity = item.Stock,
                    ProductStatus = "Active"
                };
                context.Sellerproducts.Add(sellerProd);
            }

            context.SaveChanges();
        }
    }
}
