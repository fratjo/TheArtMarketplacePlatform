using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using TheArtMarketplacePlatform.BusinessLayer.Exceptions;
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
                throw new ArgumentException("Order products cannot be null or empty");
            }

            var user = await userRepository.GetUserByIdAsync(customerId);

            if (user is null || user.CustomerProfile is null)
            {
                throw new NotFoundException("Customer not found or customer profile not found");
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
                        throw new NotFoundException("Artisan not found or artisan profile not found");
                    }

                    var deliveryPartner = await userRepository.GetUserByIdAsync(deliveryPartnerId);
                    if (deliveryPartner is null || deliveryPartner.DeliveryPartnerProfile is null)
                    {
                        throw new NotFoundException("Delivery partner not found or delivery partner profile not found");
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
                            throw new NotFoundException($"Product with ID {orderProductDto.ProductId} not found");
                        }

                        if (orderProductDto.Quantity <= 0)
                        {
                            throw new ArgumentException("Quantity must be greater than zero");
                        }

                        if (orderProductDto.Quantity > product.QuantityLeft)
                        {
                            throw new ArgumentException($"Insufficient stock for product {product.Name}. Available: {product.QuantityLeft}, Requested: {orderProductDto.Quantity}");
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
                throw new InvalidOperationException("Transaction failed, rolling back", e);
            }


            return true; // Order created successfully
        }

        public async Task<Order?> GetOrderAsync(Guid customerId, Guid orderId)
        {
            var order = await orderRepository.GetOrderByIdAsync(orderId);
            if (order is null) throw new NotFoundException("Order not found");
            if (order.CustomerId != customerId)
            {
                throw new UnauthorizedAccessException("Order does not belong to the customer");
            }
            return order;
        }

        public Task<List<Order>> GetOrdersAsync(Guid customerId)
        {
            return orderRepository.GetOrdersByCustomerIdAsync(customerId);
        }

        public async Task<bool> AlreadyBoughtReviewedAsync(Guid customerId, Guid productId)
        {
            var orders = await orderRepository.GetOrdersByCustomerIdAsync(customerId);
            if (orders == null || !orders.Any())
            {
                return false; // No orders found for the customer
            }

            orders = orders.Where(o => o.Status == OrderStatus.Delivered).ToList();

            foreach (var order in orders)
            {
                var orderProducts = await orderRepository.GetOrderProductsByOrderIdAsync(order.Id);
                if (orderProducts.Any(op => op.ProductId == productId))
                {
                    // Check if the customer has reviewed the product
                    var review = await productRepository.GetReviewOfUserAsync(productId, customerId);
                    if (review is not null) return false;
                    return true; // Custome can review the product
                }
            }

            return false; // The customer has not bought or reviewed the product
        }

        public async Task<bool> ReviewProductAsync(Guid customerId, CustomerLeaveProductReviewRequest review)
        {
            var product = await productRepository.GetByIdAsync(review.ProductId);
            if (product is null)
            {
                throw new NotFoundException("Product not found");
            }

            if (review.Rating < 1 || review.Rating > 5)
            {
                throw new ArgumentOutOfRangeException("Rating must be between 1 and 5");
            }

            var existingReview = await productRepository.GetReviewOfUserAsync(review.ProductId, customerId);
            if (existingReview is not null)
            {
                throw new InvalidOperationException("You have already reviewed this product");
            }

            var newReview = new ProductReview
            {
                Id = Guid.NewGuid(),
                ProductId = review.ProductId,
                CustomerId = customerId,
                Rating = review.Rating,
                CustomerComment = review.Review,
                CreatedAt = DateTime.Now,
            };

            await productRepository.CreateReviewAsync(newReview);

            // Update product rating
            var reviews = await productRepository.GetReviewsByProductIdAsync(review.ProductId);
            if (reviews.Any())
            {
                var averageRating = reviews.Average(r => r.Rating);
                product.Rating = (decimal)averageRating;
                await productRepository.UpdateAsync(product);
            }

            return true; // Review added successfully
        }

        public async Task<CustomerProfileResponse?> GetCustomerAsync(Guid customerId)
        {
            var user = await userRepository.GetUserByIdAsync(customerId);
            if (user is null || user.CustomerProfile is null)
            {
                throw new NotFoundException("Customer not found or customer profile not found");
            }

            return new CustomerProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                ShippingAddress = user.CustomerProfile.ShippingAddress
                // Mappe les autres propriétés si besoin
            };
        }

        public async Task<bool> CheckEmailExistsAsync(string email) => await userRepository.GetUserByEmailAsync(email) is not null;
        public async Task<bool> CheckUsernameExistsAsync(string username) => await userRepository.GetUserByUsernameAsync(username) is not null;

        public async Task<bool> UpdateCustomerAsync(Guid customerId, CustomerUpdateProfileRequest request)
        {
            var user = await userRepository.GetUserByIdAsync(customerId);
            if (user is null || user.CustomerProfile is null)
            {
                throw new NotFoundException("Artisan not found");
            }

            if (await userRepository.GetUserByEmailAsync(request.Email!) is not null && user.Email != request.Email)
            {
                throw new InvalidCredentialsException("Email already exists");
            }

            if (await userRepository.GetUserByUsernameAsync(request.Username!) is not null && user.Username != request.Username)
            {
                throw new InvalidCredentialsException("Username already exists");
            }

            user.CustomerProfile.ShippingAddress = request.ShippingAddress!;
            user.Username = request.Username!;
            user.Email = request.Email!;
            user.UpdatedAt = DateTime.UtcNow;

            await userRepository.UpdateUserAsync(user);

            return true;
        }

        public async Task<List<CustomerFavoriteProductResponse>> GetFavoriteProductsAsync(Guid customerId)
        {
            var favorites = await productRepository.GetFavoritesByUserIdAsync(customerId);
            return favorites.Select(f => new CustomerFavoriteProductResponse
            {
                Id = f.Id,
                Name = f.Name,
                Description = f.Description,
                Price = f.Price,
                Category = f.Category!.Name,
                Availability = f.Availability.ToString(), // Assuming Category is a navigation property
                Rating = f.Rating,
                ImageUrl = f.ImageUrl, // Assuming ImageUrl is a property of Product
            }).ToList();
        }

        public async Task<bool> AddProductToFavoritesAsync(Guid customerId, Guid productId)
        {
            var product = await productRepository.GetByIdAsync(productId);
            if (product is null || product.IsDeleted)
            {
                throw new NotFoundException("Product not found or is deleted");
            }

            var favorite = await productRepository.GetFavoritesByUserIdAsync(customerId);
            if (favorite.Any(f => f.Id == productId)) return true;

            return await productRepository.AddToFavoritesAsync(customerId, productId);
        }

        public Task<bool> RemoveProductFromFavoritesAsync(Guid customerId, Guid productId)
        {
            return productRepository.RemoveFromFavoritesAsync(customerId, productId);
        }
    }
}