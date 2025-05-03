using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.Core.DTOs
{
    public class ArtisanInsertProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityLeft { get; set; } = 0;
        public string Category { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? ImageExtension { get; set; } = string.Empty;
    }

    public class ArtisanUpdateProductRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int QuantityLeft { get; set; } = 0;
        public string Category { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string? ImageExtension { get; set; } = string.Empty;
    }

    public class ArtisanProductResponse
    {
    }
}