using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public UserStatus Status { get; set; } = UserStatus.Active;
        public UserRole Role { get; set; } = UserRole.Customer;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; } = null;

        public ArtisanProfile? ArtisanProfile { get; set; }
        public CustomerProfile? CustomerProfile { get; set; }
        public DeliveryPartnerProfile? DeliveryPartnerProfile { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }

    public enum UserRole
    {
        Admin,
        Artisan,
        Customer,
        DeliveryPartner
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Banned
    }

    public class ArtisanProfile
    {
        public Guid UserId { get; set; }
        public string Bio { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public ICollection<Product> Products { get; set; } = null!;
        public ICollection<ProductReview> ProductReviews { get; set; } = null!;
    }

    public class CustomerProfile
    {
        public Guid UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public ICollection<ProductFavorite> FavoriteProducts { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = null!;
        public ICollection<ProductReview> ProductReviews { get; set; } = null!;
    }

    public class DeliveryPartnerProfile
    {
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public User User { get; set; } = null!;
        public ICollection<Order> Orders { get; set; } = null!;
    }

    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(30);
        public bool IsRevoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}