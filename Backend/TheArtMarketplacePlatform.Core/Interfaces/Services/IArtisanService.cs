using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IArtisanService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(Guid? ArsitsanId, string? search = null,
            string? category = null, string? status = null, string? availability = null, decimal? rating = null, string? sortBy = null, string? sortOrder = null);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<byte[]?> GetProductImageAsync(Guid id);
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
        Task<Product> CreateProductAsync(Guid ArsitsanId, ArtisanInsertProductRequest request);
        Task<Product> UpdateProductAsync(Guid ArsitsanId, Guid ProductId, ArtisanUpdateProductRequest request);
        Task<bool> DeleteProductAsync(Guid ArsitsanId, Guid id);
    }
}