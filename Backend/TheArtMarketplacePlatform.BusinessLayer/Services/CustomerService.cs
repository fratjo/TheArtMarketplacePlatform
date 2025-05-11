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
    public class CustomerService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository
    ) : ICustomerService
    {
        public async Task<bool> CreateOrderAsync(Guid customerId, List<CustomerInsertOrderProductDto> orderProducts)
        {
            if (orderProducts == null || !orderProducts.Any())
            {
                return false; // TODO handle empty order products
            }

            var user = await userRepository.GetUserByIdAsync(customerId);

            if (user is null || user.CustomerProfile is null)
            {
                return false; // TODO handle user not found
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ShippingAddress = user!.CustomerProfile!.ShippingAddress,
                Status = OrderStatus.Pending,
            };

            var orderProductEntities = new List<OrderProduct>();

            // Process each order product sequentially
            foreach (var orderProductDto in orderProducts)
            {
                var product = await productRepository.GetByIdAsync(orderProductDto.ProductId);
                if (product is null)
                {
                    continue; // TODO handle product not found
                }

                var artisan = await userRepository.GetUserByIdAsync(product.ArtisanId);
                if (artisan is null || artisan.ArtisanProfile is null)
                {
                    continue; // TODO handle artisan not found
                }

                if (orderProductDto.Quantity <= 0)
                {
                    continue; // TODO handle invalid quantity
                }

                var orderProductEntity = new OrderProduct
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    ProductId = product.Id,
                    ArtisanName = artisan.Username,
                    ProductName = product.Name,
                    ProductDescription = product.Description,
                    ProductPrice = product.Price,
                    Quantity = orderProductDto.Quantity,
                };

                orderProductEntities.Add(orderProductEntity);
            }

            try
            {
                await orderRepository.CreateOrderAsync(order);

                foreach (var orderProduct in orderProductEntities)
                {
                    await orderRepository.CreateOrderProductAsync(orderProduct);
                }

                await orderRepository.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to create order", e);
            }
        }
    }
}