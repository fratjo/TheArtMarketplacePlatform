using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.DTOs
{
    public class RegisterRequest
    {
        // Common part 
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class RegisterArtisanRequest : RegisterRequest
    {
        // Artisan part
        public string Bio { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class RegisterCustomerRequest : RegisterRequest
    {
        // Customer part
        public string ShippingAddress { get; set; } = string.Empty;
    }

    public class RegisterDeliveryPartnerRequest : RegisterRequest
    {
        // Delivery partner-specific properties can be added here
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;
    }
}