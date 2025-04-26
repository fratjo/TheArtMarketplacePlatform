using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetByArtisanIdAsync(Guid artisanId);
        Task AddAsync(Product product);
        Task AddCategoryAsync(ProductCategory category);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Guid id);

        // Query methods
        Task<Guid?> DoesCategoryExistAsync(string name);
        Task<Guid?> DoesProductExistAsync(Guid productId);
    }
}