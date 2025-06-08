using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;

namespace TheArtMarketplacePlatform.DataAccessLayer.Repositories
{
    public class UserRepository(TheArtMarketplacePlatformDbContext _context) : IUserRepository
    {
        #region Save

        public async Task<User> SaveUserAsync(User user)
        {
            _context.Users.Add(user);
            return await _context.SaveChangesAsync().ContinueWith(t => user);
        }

        public async Task<CustomerProfile> SaveCustomerProfileAsync(CustomerProfile customerProfile)
        {
            _context.CustomerProfiles.Add(customerProfile);
            return await _context.SaveChangesAsync().ContinueWith(t => customerProfile);
        }

        #endregion

        #region Get

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                            .Include(u => u.CustomerProfile)
                            .Include(u => u.ArtisanProfile)
                            .Include(u => u.DeliveryPartnerProfile)
                            .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                            .Include(u => u.CustomerProfile)
                            .Include(u => u.ArtisanProfile)
                            .Include(u => u.DeliveryPartnerProfile)
                            .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<IEnumerable<User>> GetAllArtisansAsync()
        {
            return await _context.Users
                            .Include(u => u.ArtisanProfile)
                            .Where(u => u.Role == UserRole.Artisan)
                            .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllDeliveryPartnersAsync()
        {
            return await _context.Users
                            .Include(u => u.DeliveryPartnerProfile)
                            .Where(u => u.Role == UserRole.DeliveryPartner)
                            .ToListAsync();
        }

        public Task<User?> GetUserByIdAsync(Guid id)
        {
            return _context.Users
                            .Include(u => u.CustomerProfile)
                            .Include(u => u.ArtisanProfile)
                            .Include(u => u.DeliveryPartnerProfile)
                            .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return await _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
        }

        public async Task SaveRefreshTokenAsync(RefreshToken newToken)
        {
            _context.RefreshTokens.Add(newToken);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<RefreshToken>> GetRefreshTokensByUserIdAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> UpdateRefreshTokenAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            return await _context.SaveChangesAsync().ContinueWith(t => t.Result > 0);
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            return await _context.RefreshTokens
                .Where(rt => rt.Token == refreshToken)
                .Select(rt => rt.User)
                .FirstOrDefaultAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.CustomerProfile)
                .Include(u => u.ArtisanProfile)
                .Include(u => u.DeliveryPartnerProfile)
                .ToListAsync();
        }

        #endregion
    }
}