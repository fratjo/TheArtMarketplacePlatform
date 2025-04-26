using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(Guid? ArsitsanId);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> CreateProductAsync(Guid ArsitsanId, ArtisanInsertProductRequest request);
        Task<Product> UpdateProductAsync(Guid ArsitsanId, ArtisanUpdateProductRequest request);
        Task<bool> DeleteProductAsync(Guid ArsitsanId, Guid id);
    }
}