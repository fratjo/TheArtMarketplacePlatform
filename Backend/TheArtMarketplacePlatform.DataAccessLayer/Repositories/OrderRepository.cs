using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        }

        // This method is used to save changes to the database context.
        // It is called after creating an order and its products to persist the changes.
        // Permit to save all changes made in the context to the database.
        // Transactional behavior is handled by the DbContext.
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}