using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<UserFullProfileResponse>> GetAllUsersAsync(
            string? search = null,
            string? status = null,
            string? role = null,
            bool? isDeleted = null,
            string? sortBy = null,
            string? sortOrder = null);

        Task<IEnumerable<ProductResponse>> GetAllProductsAsync(
            string? search = null,
            string? artisan = null,
            string? category = null,
            string? sortBy = null,
            string? sortOrder = null);

        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> DeactivateUserAsync(Guid userId);
        Task<bool> DeleteProductAsync(Guid productId);
    }
}