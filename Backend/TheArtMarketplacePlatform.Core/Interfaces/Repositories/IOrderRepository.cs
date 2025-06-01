using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task CreateOrderAsync(Order order);
        Task CreateOrderProductAsync(OrderProduct orderProduct);
        Task CreateDeliveryStatusUpdateAsync(DeliveryStatusUpdate deliveryStatusUpdate);
        Task SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<Order?> GetOrderByIdAsync(Guid orderId);
        Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId);
        Task<List<Order>> GetOrdersByArtisanIdAsync(Guid artisanId);
        Task<List<Order>>
        GetDeliveryOrdersByDeliveryPartnerAsync(Guid deliveryPartnerId);
        Task UpdateOrderAsync(Order order);
    }
}