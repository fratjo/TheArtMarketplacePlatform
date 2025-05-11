using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.Core.DTOs;

namespace TheArtMarketplacePlatform.Core.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<bool> CreateOrderAsync(Guid customerId, List<CustomerInsertOrderProductDto> orderProducts);
    }
}