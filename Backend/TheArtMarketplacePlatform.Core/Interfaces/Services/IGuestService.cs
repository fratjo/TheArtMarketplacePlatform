using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IGuestService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync(
            string? search = null,
            string? artisans = null,
            string? categories = null,
            string? status = null,
            string? availability = null,
            decimal? rating = null,
            string? sortBy = null,
            string? sortOrder = null);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllArtisansAsync();
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
    }
}