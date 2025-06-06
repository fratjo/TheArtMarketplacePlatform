using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.Core.DTOs
{
    public class SimpleUserResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Role { get; set; }
    }

    public class CustomerProfileResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ShippingAddress { get; set; }
    }

    public class CustomerUpdateProfileRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? ShippingAddress { get; set; }
    }

    public class ArtisanProfileResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
    }

    public class ArtisanUpdateProfileRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Bio { get; set; }
        public string? City { get; set; }
    }

    public class DeliveryPartnerProfileResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
    }

    public class DeliveryPartnerUpdateProfileRequest
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}