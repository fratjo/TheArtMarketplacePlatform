using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetByArtisanIdAsync(Guid artisanId);
        Task<ProductReview?> GetReviewOfUserAsync(Guid productId, Guid userId);
        Task AddAsync(Product product);
        Task AddCategoryAsync(ProductCategory category);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);
        Task CreateReviewAsync(ProductReview review);
        Task<ProductReview?> GetReviewByIdAsync(Guid reviewId);
        Task UpdateReviewAsync(ProductReview review);
        Task<List<ProductReview>> GetReviewsByProductIdAsync(Guid productId);
        Task<List<Product>> GetFavoritesByUserIdAsync(Guid userId);
        Task<bool> AddToFavoritesAsync(Guid userId, Guid productId);

        // Query methods
        Task<Guid?> DoesCategoryExistAsync(string name);
        Task<Guid?> DoesProductExistAsync(Guid productId);
    }
}