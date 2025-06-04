using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;

namespace TheArtMarketplacePlatform.DataAccessLayer.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TheArtMarketplacePlatformDbContext _dbContext;

        public ProductRepository(TheArtMarketplacePlatformDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddCategoryAsync(ProductCategory category)
        {
            await _dbContext.ProductCategories.AddAsync(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var product = await _dbContext.Products.FindAsync(id);
            if (product != null)
            {
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Guid?> DoesCategoryExistAsync(string name)
        {
            var category = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(c => c.Name == name);
            return category?.Id;
        }

        public async Task<Guid?> DoesProductExistAsync(Guid productId)
        {
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.Id == productId);
            return product?.Id;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products
                .Include(p => p.Artisan)
                .Include(p => p.Category)
                .Include(p => p.ProductReviews)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            return await _dbContext.ProductCategories
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByArtisanIdAsync(Guid artisanId)
        {
            return await _dbContext.Products
                .Where(p => p.ArtisanId == artisanId)
                .Include(p => p.Artisan).ThenInclude(a => a.User)
                .Include(p => p.Category)
                .Include(p => p.ProductReviews)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Artisan).ThenInclude(a => a.User)
                .Include(p => p.ProductReviews).ThenInclude(pr => pr.Customer)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task UpdateAsync(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ProductReview?> GetReviewOfUserAsync(Guid productId, Guid userId)
        {
            return await _dbContext.ProductReviews
                .Where(pr => pr.ProductId == productId && pr.CustomerId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task CreateReviewAsync(ProductReview review)
        {
            await _dbContext.ProductReviews.AddAsync(review);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ProductReview>> GetReviewsByProductIdAsync(Guid productId)
        {
            return await _dbContext.ProductReviews
                .Where(pr => pr.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductReview?> GetReviewByIdAsync(Guid reviewId)
        {
            return await _dbContext.ProductReviews
                .Include(pr => pr.Customer)
                .Include(pr => pr.Product).ThenInclude(p => p.Artisan)
                .FirstOrDefaultAsync(pr => pr.Id == reviewId);
        }

        public async Task UpdateReviewAsync(ProductReview review)
        {
            _dbContext.ProductReviews.Update(review);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Product>> GetFavoritesByUserIdAsync(Guid userId)
        {
            var p = await _dbContext.Products
                .Where(p => p.IsDeleted == false && p.Artisan.UserId == userId)
                .Include(p => p.Artisan).ThenInclude(a => a.User)
                .Include(p => p.Category)
                .Include(p => p.ProductReviews)
                .ToListAsync();

            var f = await _dbContext.ProductFavorites
                .Where(f => f.CustomerId == userId)
                .Select(f => f.ProductId)
                .ToListAsync();

            return p.Where(x => f.Contains(x.Id)).ToList();
        }

        public async Task<bool> AddToFavoritesAsync(Guid userId, Guid productId)
        {
            var product = await _dbContext.Products.FindAsync(productId);
            if (product == null || product.IsDeleted)
            {
                throw new KeyNotFoundException("Product not found or is deleted.");
            }

            var favorite = new ProductFavorite
            {
                CustomerId = userId,
                ProductId = productId
            };

            await _dbContext.ProductFavorites.AddAsync(favorite);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}