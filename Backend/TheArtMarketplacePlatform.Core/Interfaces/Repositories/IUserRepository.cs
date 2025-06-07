using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> SaveUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllArtisansAsync();
        Task<IEnumerable<User>> GetAllDeliveryPartnersAsync();
        Task<bool> UpdateUserAsync(User user);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task SaveRefreshTokenAsync(RefreshToken newToken);
        Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserIdAsync(Guid userId);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateRefreshTokenAsync(RefreshToken token);

    }
}