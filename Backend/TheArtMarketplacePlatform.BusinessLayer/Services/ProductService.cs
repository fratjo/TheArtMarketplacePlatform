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
    public class ProductService(IProductRepository _productRepository) : IProductService
    {
        public async Task<Product> CreateProductAsync(Guid ArsitsanId, ArtisanInsertProductRequest request)
        {
            // check if category exists
            var categoryExists = await _productRepository.DoesCategoryExistAsync(request.Category);
            if (categoryExists == null)
            {
                // create category
                categoryExists = await AddCategoryAsync(request.Category);
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
                ArtisanId = ArsitsanId
            };

            await _productRepository.AddAsync(product);
            return product;
        }

        private async Task<Guid> AddCategoryAsync(string categoryName)
        {
            var formattedCategoryName = string.Join(" ", categoryName.Split(' ')
                .Select(word => new[] { "in", "of" }.Contains(word.ToLower())
                    ? word.ToLower()
                    : char.ToUpper(word[0]) + word.Substring(1).ToLower()));

            var category = new ProductCategory
            {
                Id = Guid.NewGuid(),
                Name = formattedCategoryName,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _productRepository.AddCategoryAsync(category);
            return category.Id;
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

        public async Task<IEnumerable<Product>> GetAllProductsAsync(Guid? id)
        {
            if (id == null)
            {
                return await _productRepository.GetAllAsync();
            }

            var products = await _productRepository.GetByArtisanIdAsync(id.Value);
            return products;
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
            product.Availability = request.IsAvailable ? ProductAvailability.Available : ProductAvailability.Unavailable;
            product.Status = request.QuantityLeft > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock;

            if (request.Category != null)
            {
                var categoryExists = await _productRepository.DoesCategoryExistAsync(request.Category);
                if (categoryExists == null)
                {
                    categoryExists = await AddCategoryAsync(request.Category);
                }
                product.CategoryId = categoryExists;
            }

            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return product;
        }
    }
}