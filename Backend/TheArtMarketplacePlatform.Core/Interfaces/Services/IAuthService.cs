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
        Task<AuthResponse> RegisterArtisanAsync(RegisterArtisanRequest request);
        Task<AuthResponse> RegisterCustomerAsync(RegisterCustomerRequest request);
        Task<AuthResponse> RegisterDeliveryPartnerAsync(RegisterDeliveryPartnerRequest request);
        Task<AuthResponse> LoginUserAsync(LoginRequest request);
        Task<AuthResponse?> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutUserAsync(string refreshToken);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }
}