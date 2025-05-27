using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.Core.DTOs
{
    public class CustomerInsertOrderRequest
    {
        public Guid CustomerId { get; set; }
        public Guid DeliveryPartnerId { get; set; }
        public List<CustomerInsertOrderProductDto> OrderProducts { get; set; } = new();
    }

    public class CustomerInsertOrderProductDto
    {
        public Guid ProductId { get; set; }
        public Guid ArtisanId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class ArtisanUpdateOrderStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }
}