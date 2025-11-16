using CarPartsSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace CarPartsSystem.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Create roles
            string[] roleNames = { "Admin", "Staff", "Member" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create admin user
            var adminEmail = "admin@carparts.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create staff user
            var staffEmail = "staff@carparts.com";
            var staffUser = await userManager.FindByEmailAsync(staffEmail);

            if (staffUser == null)
            {
                staffUser = new ApplicationUser
                {
                    UserName = staffEmail,
                    Email = staffEmail,
                    FirstName = "Staff",
                    LastName = "User",
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                var result = await userManager.CreateAsync(staffUser, "Staff123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(staffUser, "Staff");
                }
            }

            // Seed payment methods
            if (!context.PaymentMethods.Any())
            {
                context.PaymentMethods.AddRange(
                    new PaymentMethod { MethodName = "Cash", Description = "Cash payment", IsActive = true },
                    new PaymentMethod { MethodName = "Bank Transfer", Description = "Bank transfer payment", IsActive = true },
                    new PaymentMethod { MethodName = "Credit Card", Description = "Credit card payment", IsActive = true },
                    new PaymentMethod { MethodName = "QR Code", Description = "QR code payment (PromptPay)", IsActive = true }
                );
                await context.SaveChangesAsync();
            }

            // Seed contact info
            if (!context.ContactInfos.Any())
            {
                context.ContactInfos.Add(new ContactInfo
                {
                    CompanyName = "Car Parts System",
                    Address = "123 Main Street, Bangkok, Thailand",
                    Phone = "+66-12-345-6789",
                    Email = "info@carparts.com",
                    FacebookUrl = "https://facebook.com/carparts",
                    LineId = "@carparts",
                    InstagramUrl = "https://instagram.com/carparts",
                    UpdatedAt = DateTime.Now
                });
                await context.SaveChangesAsync();
            }

            // Seed sample parts
            if (!context.Parts.Any())
            {
                context.Parts.AddRange(
                    new Part
                    {
                        PartName = "Brake Pad Set",
                        Description = "High-quality brake pads for various car models",
                        Price = 1500.00m,
                        StockQuantity = 50,
                        Category = "Brakes",
                        Brand = "Brembo",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    },
                    new Part
                    {
                        PartName = "Engine Oil Filter",
                        Description = "Premium oil filter for engine protection",
                        Price = 250.00m,
                        StockQuantity = 100,
                        Category = "Engine",
                        Brand = "Bosch",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    },
                    new Part
                    {
                        PartName = "Air Filter",
                        Description = "High-efficiency air filter",
                        Price = 450.00m,
                        StockQuantity = 75,
                        Category = "Engine",
                        Brand = "Mann Filter",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    },
                    new Part
                    {
                        PartName = "Spark Plugs Set",
                        Description = "Set of 4 spark plugs",
                        Price = 800.00m,
                        StockQuantity = 60,
                        Category = "Engine",
                        Brand = "NGK",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    },
                    new Part
                    {
                        PartName = "Wiper Blades",
                        Description = "Front wiper blades pair",
                        Price = 350.00m,
                        StockQuantity = 40,
                        Category = "Accessories",
                        Brand = "Bosch",
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    }
                );
                await context.SaveChangesAsync();
            }

            // Seed promotions
            if (!context.Promotions.Any())
            {
                context.Promotions.AddRange(
                    new Promotion
                    {
                        Title = "New Year Sale",
                        Description = "10% off on all engine parts",
                        DiscountPercentage = 10.00m,
                        MinimumPurchaseAmount = 1000.00m,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(1),
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    },
                    new Promotion
                    {
                        Title = "Weekend Special",
                        Description = "15% off on purchases over 5000 THB",
                        DiscountPercentage = 15.00m,
                        MinimumPurchaseAmount = 5000.00m,
                        StartDate = DateTime.Now,
                        EndDate = DateTime.Now.AddMonths(3),
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}
