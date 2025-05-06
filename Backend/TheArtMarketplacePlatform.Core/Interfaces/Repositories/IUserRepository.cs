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
        Task<IEnumerable<User>> GetAllArtisansAsync();
    }
}