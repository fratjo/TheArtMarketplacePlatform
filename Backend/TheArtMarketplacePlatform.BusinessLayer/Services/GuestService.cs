using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class GuestService(IProductRepository productRepository, IUserRepository userRepository, IWebHostEnvironment _env) : IGuestService
    {
        public async Task<IEnumerable<SimpleUserResponse>> GetAllArtisansAsync()
        {
            var artisans = await userRepository.GetAllArtisansAsync();
            return artisans.Select(a => new SimpleUserResponse
            {
                Id = a.Id,
                Username = a.Username,
                Email = a.Email,
                Role = a.Role.ToString()
            });
        }

        public async Task<IEnumerable<SimpleUserResponse>> GetAllDeliveryPartnersAsync()
        {
            var deliveryPartners = await userRepository.GetAllDeliveryPartnersAsync();
            return deliveryPartners.Select(dp => new SimpleUserResponse
            {
                Id = dp.Id,
                Username = dp.Username,
                Email = dp.Email,
                Role = dp.Role.ToString()
            });
        }

        public async Task<IEnumerable<ProductCategoryResponse>> GetAllCategoriesAsync()
        {
            var categories = await productRepository.GetAllCategoriesAsync();
            return categories.Select(c => new ProductCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync(string? search = null, string? artisans = null, string? categories = null, string? status = null, string? availability = null, string? rating = null, string? sortBy = null, string? sortOrder = null)
        {

            var products = await productRepository.GetAllAsync();

            products = products.Where(p => !p.IsDeleted && p.Availability == ProductAvailability.Available);

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Category != null && p.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    p.Status.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Availability.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(artisans))
            {
                var artisanList = artisans.Split(',').Select(a => a.Trim()).ToList();
                products = products.Where(p => p.Artisan != null && artisanList.Contains(p.Artisan.UserId.ToString(), StringComparer.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(categories))
            {
                var categoryList = categories.Split(',').Select(c => c.Trim()).ToList();
                products = products.Where(p => p.Category != null && categoryList.Contains(p.Category.Id.ToString(), StringComparer.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(status))
            {
                products = products.Where(p => p.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(availability))
            {
                products = products.Where(p => p.Availability.ToString().Equals(availability, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(rating))
            {
                var ratingList = rating.Split(',').Select(r => r.Trim()).ToList();
                products = products.Where(p => ratingList.Any(r => Math.Round((decimal)p.Rating!) >= Convert.ToInt32(r)));
                foreach (var p in products)
                {
                    Console.WriteLine(p.Rating);
                }
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                products = sortBy.ToLower() switch
                {
                    "name" => sortOrder == "desc" ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                    "price" => sortOrder == "desc" ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                    "rating" => sortOrder == "desc"
                        ? products.OrderByDescending(p => p.Rating ?? 0)
                        : products.OrderBy(p => p.Rating ?? 0),
                    _ => products
                };
            }

            return products.Select(p => new ProductResponse
            {
                Id = p.Id,
                ArtisanId = p.ArtisanId,
                ArtisanName = p.Artisan?.User?.Username ?? "Unknown Artisan",
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                QuantityLeft = p.QuantityLeft,
                Category = p.Category?.Name ?? "Uncategorized",
                Availability = p.Availability.ToString(),
                Rating = p.Rating ?? 0,
                ImageUrl = p.ImageUrl,
                ProductReviews = p.ProductReviews.ToList(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            });
        }

        public async Task<ProductResponse> GetProductByIdAsync(Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            return new ProductResponse
            {
                Id = product.Id,
                ArtisanId = product.ArtisanId,
                ArtisanName = product.Artisan?.User.Username ?? "Unknown Artisan",
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityLeft = product.QuantityLeft,
                Category = product.Category?.Name ?? "Uncategorized",
                Availability = product.Availability.ToString(),
                Rating = product.Rating,
                ImageUrl = product.ImageUrl,
                ProductReviews = product.ProductReviews.ToList(),
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<byte[]?> GetProductImageAsync(Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null || string.IsNullOrEmpty(product.ImageUrl))
            {
                return null;
            }

            var imagePath = Path.Combine(_env.WebRootPath, product.ImageUrl);
            if (!File.Exists(imagePath))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(imagePath);
        }
    }
}