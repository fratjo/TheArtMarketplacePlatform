using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IGuestService
    {
        Task<IEnumerable<ProductResponse>> GetAllProductsAsync(
            string? search = null,
            string? artisans = null,
            string? categories = null,
            string? status = null,
            string? availability = null,
            string? rating = null,
            string? sortBy = null,
            string? sortOrder = null);
        Task<ProductResponse> GetProductByIdAsync(Guid id);
        Task<IEnumerable<SimpleUserResponse>> GetAllArtisansAsync();
        Task<IEnumerable<SimpleUserResponse>> GetAllDeliveryPartnersAsync();
        Task<IEnumerable<ProductCategoryResponse>> GetAllCategoriesAsync();
    }
}