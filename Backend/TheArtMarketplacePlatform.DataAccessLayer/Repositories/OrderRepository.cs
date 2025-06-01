using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;

namespace TheArtMarketplacePlatform.DataAccessLayer.Repositories
{
    public class OrderRepository(TheArtMarketplacePlatformDbContext context) : IOrderRepository
    {
        public async Task CreateOrderAsync(Order order)
        {
            await context.Orders.AddAsync(order);
        }

        public async Task CreateOrderProductAsync(OrderProduct orderProduct)
        {
            await context.OrderProducts.AddAsync(orderProduct);

            var existingProduct = await context.Products
                .FirstOrDefaultAsync(p => p.Id == orderProduct.ProductId);
            if (existingProduct != null)
            {
                existingProduct.QuantityLeft -= orderProduct.Quantity;
            }
            else
            {
                throw new InvalidOperationException("Product not found for order product.");
            }
        }

        public async Task CreateDeliveryStatusUpdateAsync(DeliveryStatusUpdate deliveryStatusUpdate)
        {
            await context.DeliveryStatusUpdates.AddAsync(deliveryStatusUpdate);
        }

        public async Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            return await context.Orders
                .Where(o => o.Id == orderId)
                .Include(o => o.OrderProducts)
                .Include(o => o.DeliveryStatusUpdates)
                .Include(o => o.Customer).ThenInclude(c => c.User)
                .Include(o => o.DeliveryPartner).ThenInclude(dp => dp.User)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetOrdersByCustomerIdAsync(Guid customerId)
        {
            return await context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderProducts)
                .Include(o => o.DeliveryStatusUpdates)
                .Include(o => o.DeliveryPartner).ThenInclude(dp => dp.User)
                .ToListAsync();
        }

        public async Task<List<OrderProduct>> GetOrderProductsByOrderIdAsync(Guid orderId)
        {
            return await context.OrderProducts
                .Where(op => op.OrderId == orderId)
                .Include(op => op.Product)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (context.Database.CurrentTransaction == null)
            {
                await context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (context.Database.CurrentTransaction != null)
            {
                await context.SaveChangesAsync();
                await context.Database.CommitTransactionAsync();
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (context.Database.CurrentTransaction != null)
            {
                await context.Database.RollbackTransactionAsync();
            }
        }

        public async Task<List<Order>> GetOrdersByArtisanIdAsync(Guid artisanId)
        {
            var query = context.Orders
                .Where(o => o.ArtisanId == artisanId)
                .Include(o => o.OrderProducts)
                .Include(o => o.DeliveryStatusUpdates)
                .Include(o => o.DeliveryPartner).ThenInclude(dp => dp.User);

            return await query.ToListAsync();
        }

        public async Task<List<Order>> GetDeliveryOrdersByDeliveryPartnerAsync(Guid deliveryPartnerId)
        {
            var query = context.Orders
                .Where(o => o.DeliveryPartnerId == deliveryPartnerId)
                .Include(o => o.OrderProducts)
                .Include(o => o.DeliveryStatusUpdates)
                .Include(o => o.Customer).ThenInclude(c => c.User);

            return await query.ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            // Update the order in the context
            context.Orders.Update(order);
        }
    }
}