using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class DeliveryPartnerService(IOrderRepository orderRepository) : IDeliveryPartnerService
    {
        public async Task<ICollection<DeliveryPartnerDeliveryOrderResponse>> GetDeliveryOrdersByDeliveryPartnerAsync(Guid deliveryPartnerId, string? status = null, string? sortBy = null, string? sortOrder = null)
        {
            var orders = await orderRepository.GetDeliveryOrdersByDeliveryPartnerAsync(
                deliveryPartnerId);

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                orders = sortBy.ToLower() switch
                {
                    "createdat" => sortOrder == "desc" ? orders.OrderByDescending(o => o.CreatedAt).ToList() : orders.OrderBy(o => o.CreatedAt).ToList(),
                    "status" => sortOrder == "desc" ? orders.OrderByDescending(o => o.Status).ToList() : orders.OrderBy(o => o.Status).ToList(),
                    _ => orders
                };
            }

            // MAP orders to DeliveryPartnerDeliveryOrderResponse DTO
            var mappedOrders = orders.Select(order => new DeliveryPartnerDeliveryOrderResponse
            {
                OrderId = order.Id,
                CustomerName = order.Customer?.User.Username ?? "Unknown",
                ShippingAddress = order.ShippingAddress,
                Status = order.Status.ToString(),
                DeliveryStatusUpdates = order.DeliveryStatusUpdates?.Select(update => new DeliveryStatusUpdate
                {
                    Id = update.Id,
                    OrderId = update.OrderId,
                    Status = update.Status,
                    CreatedAt = update.CreatedAt
                }).ToList().OrderBy(update => update.CreatedAt).ToList() ?? new List<DeliveryStatusUpdate>(),
            }).Where(order => order.DeliveryStatusUpdates != null && order.DeliveryStatusUpdates.Any()).ToList();

            return mappedOrders;
        }

        public async Task<bool> SetDeliveryAsDeliveredAsync(Guid deliveryPartnerId, Guid deliveryId)
        {
            var order = await orderRepository.GetOrderByIdAsync(deliveryId);
            if (order == null || order.DeliveryStatusUpdates.Any(update => update.Status == DeliveryStatus.Delivered))
            {
                return false; // Order not found or already delivered
            }

            var t = orderRepository.BeginTransactionAsync();
            try
            {
                // Update the order status to Delivered
                order.Status = OrderStatus.Delivered;

                // Create a delivery status update
                var deliveryStatusUpdate = new DeliveryStatusUpdate
                {
                    OrderId = order.Id,
                    Status = DeliveryStatus.Delivered,
                    CreatedAt = DateTime.UtcNow
                };

                await orderRepository.CreateDeliveryStatusUpdateAsync(deliveryStatusUpdate);
                await orderRepository.UpdateOrderAsync(order);
                await orderRepository.SaveChangesAsync();
                await orderRepository.CommitTransactionAsync();
            }
            catch
            {
                await orderRepository.RollbackTransactionAsync();
                throw;
            }
            return true;
        }
    }
}