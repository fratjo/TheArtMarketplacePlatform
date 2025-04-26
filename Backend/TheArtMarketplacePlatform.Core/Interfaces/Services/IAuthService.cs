using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> CheckUsernameExistsAsync(string username);
        Task<string> RegisterArtisanAsync(RegisterArtisanRequest request);
        Task<string> RegisterCustomerAsync(RegisterCustomerRequest request);
        Task<string> RegisterDeliveryPartnerAsync(RegisterDeliveryPartnerRequest request);
        Task<string> LoginUserAsync(LoginRequest request);
    }
}