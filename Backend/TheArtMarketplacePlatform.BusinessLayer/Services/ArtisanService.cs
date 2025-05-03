using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class ArtisanService(IProductRepository _productRepository) : IArtisanService
    {
        public async Task<Product> CreateProductAsync(Guid ArsitsanId, ArtisanInsertProductRequest request)
        {
            // check if category exists
            var categoryExists = await _productRepository.DoesCategoryExistAsync(request.Category);

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                QuantityLeft = request.QuantityLeft,
                CategoryId = categoryExists,
                Status = request.QuantityLeft > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock,
                Availability = request.Availability == "available" ? ProductAvailability.Available : ProductAvailability.Unavailable,
                ArtisanId = ArsitsanId
            };

            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<bool> DeleteProductAsync(Guid ArsitsanId, Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.ArtisanId != ArsitsanId)
            {
                return false;
            }

            product.IsDeleted = true; // soft delete
            product.DeletedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(Guid? id, string? search = null, string? category = null, string? status = null, string? availability = null, decimal? rating = null)
        {
            if (id == null)
            {
                return await _productRepository.GetAllAsync();
            }

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

        public async Task<Product> UpdateProductAsync(Guid ArtisanId, ArtisanUpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(request.Id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (product.ArtisanId != ArtisanId)
            {
                throw new Exception("You are not authorized to update this product");
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