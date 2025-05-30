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
        public async Task<bool> CreateOrderAsync(Guid customerId, Guid deliveryPartnerId, List<CustomerInsertOrderProductDto> orderProducts)
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

            // List to Dict 
            var productByArtisan = orderProducts
                .GroupBy(op => op.ArtisanId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // start transaction
            await orderRepository.BeginTransactionAsync();

            try
            {
                foreach (var group in productByArtisan)
                {
                    var artisan = await userRepository.GetUserByIdAsync(group.Key);
                    if (artisan is null || artisan.ArtisanProfile is null)
                    {
                        continue; // TODO handle artisan not found
                    }

                    var deliveryPartner = await userRepository.GetUserByIdAsync(deliveryPartnerId);
                    if (deliveryPartner is null || deliveryPartner.DeliveryPartnerProfile is null)
                    {
                        continue; // TODO handle delivery partner not found
                    }

                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        CustomerId = customerId,
                        DeliveryPartnerId = deliveryPartnerId,
                        DeliveryPartnerName = deliveryPartner.Username,
                        ArtisanId = artisan.Id,
                        ArtisanName = artisan.Username,
                        ShippingAddress = user!.CustomerProfile!.ShippingAddress,
                        Status = OrderStatus.Pending,
                    };

                    var orderProductEntities = new List<OrderProduct>();

                    // Process each order product sequentially
                    foreach (var orderProductDto in group.Value)
                    {
                        var product = await productRepository.GetByIdAsync(orderProductDto.ProductId);
                        if (product is null)
                        {
                            continue; // TODO handle product not found
                        }

                        if (orderProductDto.Quantity <= 0)
                        {
                            continue; // TODO handle invalid quantity
                        }

                        if (orderProductDto.Quantity > product.QuantityLeft)
                        {
                            continue; // TODO handle insufficient stock
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

                    await orderRepository.CreateOrderAsync(order);

                    foreach (var orderProduct in orderProductEntities)
                    {
                        await orderRepository.CreateOrderProductAsync(orderProduct);
                    }
                }
                await orderRepository.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await orderRepository.RollbackTransactionAsync();
                throw new Exception("Transaction failed, rolling back", e); // TODO handle transaction failure
            }


            return true; // Order created successfully
        }

        public async Task<Order?> GetOrderAsync(Guid customerId, Guid orderId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null) throw new Exception("Order not found"); // TODO handle order not found
            if (order.CustomerId != customerId)
            {
                throw new Exception("Order does not belong to the customer"); // TODO handle order not belong to customer
            }
            return order;
        }

        public Task<List<Order>> GetOrdersAsync(Guid customerId)
        {
            return orderRepository.GetOrdersByCustomerIdAsync(customerId);
        }
    }
}