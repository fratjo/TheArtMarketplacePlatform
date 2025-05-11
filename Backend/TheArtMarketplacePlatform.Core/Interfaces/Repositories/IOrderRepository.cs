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
        Task SaveChangesAsync();
    }
}