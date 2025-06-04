using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TheArtMarketplacePlatform.Core.Entities.Order;

namespace TheArtMarketplacePlatform.Core.Entities
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ArtisanId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityLeft { get; set; } = 0;
        public decimal? Rating { get; set; } = null;
        public Guid? CategoryId { get; set; }
        public ProductStatus Status { get; set; } = ProductStatus.OutOfStock;
        public ProductAvailability Availability { get; set; } = ProductAvailability.Available;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; } = null;

        public ArtisanProfile Artisan { get; set; } = null!;
        public ICollection<OrderProduct> OrderProducts { get; set; } = null!;
        public ICollection<ProductReview> ProductReviews { get; set; } = null!;
        public ProductCategory? Category { get; set; }
    }

    public class ProductCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public ICollection<Product> Products { get; set; } = null!;
    }

    public enum ProductStatus
    {
        InStock,
        OutOfStock
    }

    public enum ProductAvailability
    {
        Available,
        Unavailable
    }

    public class ProductReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public Guid? CustomerId { get; set; }
        public int Rating { get; set; }
        public string CustomerComment { get; set; } = string.Empty;
        public string ArtisanResponse { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        public Product Product { get; set; } = null!;
        public CustomerProfile? Customer { get; set; }
    }

    public class ProductFavorite
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }

        public CustomerProfile Customer { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}