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
        public decimal Price { get; set; }
        public int QuantityLeft { get; set; } = 0;
        public ProductCategory Category { get; set; } = ProductCategory.Art;
        public ProductStatus Status { get; set; } = ProductStatus.OutOfStock;
        public bool IsAvailable { get; set; } = true; // Used to hide product from the marketplace // Temporary soft delete
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ArtisanProfile Artisan { get; set; } = null!;
        public ICollection<OrderProduct> OrderProducts { get; set; } = null!;
        public ICollection<ProductReview> ProductReviews { get; set; } = null!;
    }

    public enum ProductCategory
    {
        Art,
        Craft,
        Design,
        Photography,
        Sculpture,
        Painting,
        DigitalArt
    }

    public enum ProductStatus
    {
        Available,
        OutOfStock
    }

    public class ProductReview
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? ProductId { get; set; }

        // Customer 
        public Guid? CustomerId { get; set; }
        public string CustomerComment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid Rating { get; set; }

        // Artisan
        public string ArtisanResponse { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }

        public Product Product { get; set; } = null!;
        public CustomerProfile? Customer { get; set; }
    }
}