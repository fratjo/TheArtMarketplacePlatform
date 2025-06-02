using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<CustomerProfileResponse?> GetCustomerAsync(Guid customerId);
        Task<bool> UpdateCustomerAsync(Guid customerId, CustomerUpdateProfileRequest request);
        Task<bool> CreateOrderAsync(Guid customerId, Guid deliveryPartnerId, List<CustomerInsertOrderProductDto> orderProducts);
        Task<List<Order>> GetOrdersAsync(Guid customerId);
        Task<Order?> GetOrderAsync(Guid customerId, Guid orderId);
        Task<bool> AlreadyBoughtReviewedAsync(Guid customerId, Guid productId);
        Task<bool> ReviewProductAsync(Guid customerId, CustomerLeaveProductReviewRequest review);
    }
}