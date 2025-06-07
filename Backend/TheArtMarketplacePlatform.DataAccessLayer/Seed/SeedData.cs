using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TheArtMarketplacePlatform.Core.Utils;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.DataAccessLayer.Seed
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = serviceProvider.GetRequiredService<TheArtMarketplacePlatformDbContext>();

            // apply migrations and update the database
            context.Database.Migrate();


            Console.WriteLine("Seeding initial data...");
            Console.WriteLine("Start seeding users...");
            if (!context.Users.Any(u => u.Email == "admin@marketplace.com"))
            {
                // Seed admin user
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@marketplace.com",
                    PasswordSalt = Password.GenerateSalt(),
                    PasswordHash = Password.HashPasswordWithSalt("Password1!", Password.GenerateSalt()),
                    Role = UserRole.Admin,
                    Status = UserStatus.Active,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null
                };
                context.Users.Add(adminUser);
            }


            if (!context.Users.Any(u => u.Email == "customer@marketplace.com"))
            {
                // Seed customer user
                var id = Guid.NewGuid();
                var salt = Password.GenerateSalt();

                var customerUser = new User
                {
                    Id = id,
                    Username = "customer",
                    Email = "customer@marketplace.com",
                    PasswordSalt = salt,
                    PasswordHash = Password.HashPasswordWithSalt("Password1!", salt),
                    Role = UserRole.Customer,
                    Status = UserStatus.Active,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                    CustomerProfile = new CustomerProfile
                    {
                        UserId = id,
                        ShippingAddress = "123 Main St, City, Country",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
                context.Users.Add(customerUser);
            }

            if (!context.Users.Any(u => u.Email == "artisan@marketplace.com"))
            {
                // Seed artisan user
                var id = Guid.NewGuid();
                var salt = Password.GenerateSalt();

                var artisanUser = new User
                {
                    Id = id,
                    Username = "artisan",
                    Email = "artisan@marketplace.com",
                    PasswordSalt = salt,
                    PasswordHash = Password.HashPasswordWithSalt("Password1!", salt),
                    Role = UserRole.Artisan,
                    Status = UserStatus.Active,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                    ArtisanProfile = new ArtisanProfile
                    {
                        UserId = id,
                        Bio = "Artisan bio goes here",
                        City = "City Name",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                };
                context.Users.Add(artisanUser);
            }

            if (!context.Users.Any(u => u.Email == "deliverypartner@marketplace.com"))
            {
                // Seed delivery partner user
                var id = Guid.NewGuid();
                var salt = Password.GenerateSalt();

                var deliveryPartnerUser = new User
                {
                    Id = id,
                    Username = "deliverypartner",
                    Email = "deliverypartner@marketplace.com",
                    PasswordSalt = salt,
                    PasswordHash = Password.HashPasswordWithSalt("Password1!", salt),
                    Role = UserRole.DeliveryPartner,
                    Status = UserStatus.Active,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeletedAt = null,
                    DeliveryPartnerProfile = new DeliveryPartnerProfile
                    {
                        UserId = id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                };
                context.Users.Add(deliveryPartnerUser);
            }
            Console.WriteLine("Users seeded successfully.");
            context.SaveChanges();

            Console.WriteLine("Seeding product categories...");
            if (!context.ProductCategories.Any())
            {
                // Seed product categories
                context.ProductCategories.AddRange(new List<ProductCategory>
                {
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Paintings",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Sculptures",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    new ProductCategory
                    {
                        Id = Guid.NewGuid(),
                        Name = "Crafts",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    }
                });
            }
            context.SaveChanges();
            Console.WriteLine("Product categories seeded successfully.");

            Console.WriteLine("Seeding products...");
            if (!context.Products.Any())
            {
                // Ensure we have at least one artisan, customer, and delivery partner user
                var artisanUser = context.Users.FirstOrDefault(u => u.Role == UserRole.Artisan);
                if (artisanUser == null)
                {
                    throw new InvalidOperationException("No artisan user found to associate products with.");
                }

                var customerUser = context.Users.FirstOrDefault(u => u.Role == UserRole.Customer);
                if (customerUser == null)
                {
                    throw new InvalidOperationException("No customer user found to associate products with.");
                }

                var deliveryPartnerUser = context.Users.FirstOrDefault(u => u.Role == UserRole.DeliveryPartner);
                if (deliveryPartnerUser == null)
                {
                    throw new InvalidOperationException("No delivery partner user found to associate products with.");
                }

                // Seed products
                context.Products.AddRange(new List<Product>
                {
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        ArtisanId = artisanUser.Id,
                        Name = "Artisan Product 1",
                        Description = "Description for Artisan Product 1",
                        ImageUrl = "/images/27309047-0117-4bd2-9e3f-636710018560.png",
                        Price = 100.00m,
                        QuantityLeft = 10,
                        Rating = 0,
                        CategoryId = context.ProductCategories.OrderBy(c => c.Name).First().Id,
                        Status = ProductStatus.InStock,
                        Availability = ProductAvailability.Available,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    },
                    new Product
                    {
                        Id = Guid.NewGuid(),
                        ArtisanId = artisanUser.Id,
                        Name = "Artisan Product 2",
                        Description = "Description for Artisan Product 2",
                        ImageUrl = null,
                        Price = 100.00m,
                        QuantityLeft = 10,
                        Rating = 0,
                        CategoryId = context.ProductCategories.OrderBy(c => c.Name).Last().Id,
                        Status = ProductStatus.InStock,
                        Availability = ProductAvailability.Available,
                        IsDeleted = false,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        DeletedAt = null,
                    }
                });
            }
            context.SaveChanges();
            Console.WriteLine("Products seeded successfully.");
        }
    }
}