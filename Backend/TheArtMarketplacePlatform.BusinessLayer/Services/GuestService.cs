using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class GuestService(IProductRepository productRepository, IUserRepository userRepository, IWebHostEnvironment _env) : IGuestService
    {
        public Task<IEnumerable<User>> GetAllArtisansAsync()
        {
            var artisans = userRepository.GetAllArtisansAsync();
            return artisans;
        }

        public Task<IEnumerable<User>> GetAllDeliveryPartnersAsync()
        {
            var deliveryPartners = userRepository.GetAllDeliveryPartnersAsync();
            return deliveryPartners;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            var categories = await productRepository.GetAllCategoriesAsync();
            return categories;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(string? search = null, string? artisans = null, string? categories = null, string? status = null, string? availability = null, string? rating = null, string? sortBy = null, string? sortOrder = null)
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
                products = products.Where(p => p.Artisan != null && artisanList.Contains(p.Artisan.User.Username, StringComparer.OrdinalIgnoreCase));
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
                ratingList.ForEach(r => System.Console.WriteLine(r));
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
                    "rating" => sortOrder == "desc" ? products.OrderByDescending(p => p.Rating) : products.OrderBy(p => p.Rating),
                    _ => products
                };
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null) throw new Exception("Product not found");

            return product;
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