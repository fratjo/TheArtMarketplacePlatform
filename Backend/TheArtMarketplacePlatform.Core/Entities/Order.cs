using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheArtMarketplacePlatform.Core.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? DeliveryPartnerId { get; set; }
        public Guid? CustomerId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DeliveryPartnerProfile? DeliveryPartner { get; set; }
        public CustomerProfile? Customer { get; set; } = null!;
        public ICollection<OrderProduct> OrderProducts { get; set; } = null!;
        public ICollection<DeliveryStatusUpdate>? DeliveryStatusUpdates { get; set; }
    }

    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }

    public class OrderProduct
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public Guid? ProductId { get; set; }

        // Hard History
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; } = 1;

        public Order Order { get; set; } = null!;
        public Product? Product { get; set; }
    }

    public class DeliveryStatusUpdate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrderId { get; set; }
        public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Order Order { get; set; } = null!;
    }

    public enum DeliveryStatus
    {
        Pending,
        InTransit,
        Delivered,
        Returned
    }
}