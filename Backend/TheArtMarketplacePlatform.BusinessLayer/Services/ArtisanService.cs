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
    public class ArtisanService(IProductRepository _productRepository, IWebHostEnvironment _env) : IArtisanService
    {
        public async Task<Product> CreateProductAsync(Guid ArsitsanId, ArtisanInsertProductRequest request)
        {
            // check if category exists
            var categoryExists = await _productRepository.DoesCategoryExistAsync(request.Category);

            string? uniqueImageName = null;

            if (request.Image is not null && request.Image.Length != 0)
            {
                // Generate a unique name for the image
                uniqueImageName = Guid.NewGuid().ToString() + "." + request.ImageExtension;

                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);

                // Save the image to the file system as static files
                var imageFolder = Path.Combine(_env.WebRootPath!, "images");
                var imagePath = Path.Combine(imageFolder, uniqueImageName);

                // Ensure the directory exists
                if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                // Save the image to the file system
                await File.WriteAllBytesAsync(imagePath, Convert.FromBase64String(request.Image));
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                QuantityLeft = request.QuantityLeft,
                CategoryId = categoryExists,
                Status = request.QuantityLeft > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock,
                Availability = request.Availability == "available" ? ProductAvailability.Available : ProductAvailability.Unavailable,
                ArtisanId = ArsitsanId,
                ImageUrl = request.Image is not null && request.Image.Length != 0 ? Path.Combine("images", uniqueImageName!) : null,
            };

            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<bool> DeleteProductAsync(Guid ArsitsanId, Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.ArtisanId != ArsitsanId) return false;


            product.IsDeleted = true; // soft delete
            product.DeletedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(Guid? id, string? search = null, string? category = null, string? status = null, string? availability = null, decimal? rating = null, string? sortBy = null, string? sortOrder = null)
        {
            if (id == null) throw new ArgumentNullException(nameof(id), "Artisan ID cannot be null");

            var products = await _productRepository.GetByArtisanIdAsync(id.Value);

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
                    "category" => sortOrder == "desc" ? products.OrderByDescending(p => p.Category!.Name) : products.OrderBy(p => p.Category!.Name),
                    "status" => sortOrder == "desc" ? products.OrderByDescending(p => p.Status) : products.OrderBy(p => p.Status),
                    "quantityLeft" => sortOrder == "desc" ? products.OrderByDescending(p => p.QuantityLeft) : products.OrderBy(p => p.QuantityLeft),
                    // "rating" => sortOrder == "desc" ? products.OrderByDescending(p => p.ProductReviews!.Average(pr => pr.Rating)) : products.OrderBy(p => p.ProductReviews!.Average(pr => pr.Rating)),
                    "availability" => sortOrder == "desc" ? products.OrderByDescending(p => p.Availability) : products.OrderBy(p => p.Availability),
                    _ => products
                };
            }

            return products;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            var categories = await _productRepository.GetAllCategoriesAsync();
            return categories;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            return product;
        }

        public async Task<byte[]?> GetProductImageAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
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

        public async Task<Product> UpdateProductAsync(Guid ArtisanId, Guid ProductId, ArtisanUpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (product.ArtisanId != ArtisanId)
            {
                throw new Exception("You are not authorized to update this product");
            }

            if (request.Image is not null && request.Image.Length != 0)
            {
                // Generate a unique name for the new image
                var uniqueImageName = Guid.NewGuid().ToString() + "." + request.ImageExtension;

                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (string.IsNullOrEmpty(webRootPath))
                {
                    throw new InvalidOperationException("WebRootPath is not configured. Ensure wwwroot exists or is properly set up.");
                }

                if (!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);

                // Save the new image to the file system
                var imageFolder = Path.Combine(_env.WebRootPath!, "images");
                var newImagePath = Path.Combine(imageFolder, uniqueImageName);

                // Ensure the directory exists
                if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                await File.WriteAllBytesAsync(newImagePath, Convert.FromBase64String(request.Image));

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath!, product.ImageUrl);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Update the product's image URL
                product.ImageUrl = Path.Combine("images", uniqueImageName);
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.QuantityLeft = request.QuantityLeft;
            product.Availability = request.Availability == "available" ? ProductAvailability.Available : ProductAvailability.Unavailable;
            product.Status = request.QuantityLeft > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock;
            product.CategoryId = await _productRepository.DoesCategoryExistAsync(request.Category);
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return product;
        }
    }
}