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
    public class GuestService(IProductRepository productRepository, IWebHostEnvironment _env) : IGuestService
    {
        public async Task<IEnumerable<Product>> GetAllProductsAsync(string? search = null, string? category = null, string? status = null, string? availability = null, decimal? rating = null, string? sortBy = null, string? sortOrder = null)
        {

            var products = await productRepository.GetAllAsync();

            products = products.Where(p => !p.IsDeleted); // Exclude soft-deleted products // only admin can see soft-deleted products and restore them

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Category != null && p.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    p.Status.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Availability.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category!.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(status))
            {
                products = products.Where(p => p.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(availability))
            {
                products = products.Where(p => p.Availability.ToString().Equals(availability, StringComparison.OrdinalIgnoreCase));
            }
            // if (rating.HasValue)
            // {
            //     products = products.Where(p =>
            //         p.ProductReviews is not null &&
            //         (decimal)p.ProductReviews.Average(pr => pr.Rating) >= rating.Value);
            // }

            if (!string.IsNullOrEmpty(sortBy))
            {
                products = sortBy.ToLower() switch
                {
                    "name" => sortOrder == "desc" ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                    "price" => sortOrder == "desc" ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                    _ => products
                };
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

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