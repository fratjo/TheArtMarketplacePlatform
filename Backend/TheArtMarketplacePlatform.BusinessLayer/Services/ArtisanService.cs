using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class ArtisanService(IProductRepository _productRepository, IOrderRepository _orderRepository, IUserRepository userRepository, IWebHostEnvironment _env) : IArtisanService
    {
        #region Products
        public async Task<Product> CreateProductAsync(Guid ArsitsanId, ArtisanInsertProductRequest request)
        {
            // check if category exists
            var categoryExists = await _productRepository.DoesCategoryExistAsync(request.Category);

            string? uniqueImageName = null;

            if (request.Image is not null && request.Image.Length != 0)
            {
                // Generate a unique name for the image
                uniqueImageName = Guid.NewGuid().ToString() + "." + request.ImageExtension;

                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);

                // Save the image to the file system as static files
                var imageFolder = Path.Combine(_env.WebRootPath!, "images");
                var imagePath = Path.Combine(imageFolder, uniqueImageName);

                // Ensure the directory exists
                if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                // Save the image to the file system
                await File.WriteAllBytesAsync(imagePath, Convert.FromBase64String(request.Image));
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                QuantityLeft = request.QuantityLeft,
                CategoryId = categoryExists,
                Status = request.QuantityLeft > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock,
                Availability = request.Availability == "available" ? ProductAvailability.Available : ProductAvailability.Unavailable,
                ArtisanId = ArsitsanId,
                ImageUrl = request.Image is not null && request.Image.Length != 0 ? Path.Combine("images", uniqueImageName!) : null,
            };

            await _productRepository.AddAsync(product);
            return product;
        }

        public async Task<bool> DeleteProductAsync(Guid ArsitsanId, Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || product.ArtisanId != ArsitsanId) return false;


            product.IsDeleted = true; // soft delete
            product.DeletedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync(Guid? id, string? search = null, string? category = null, string? status = null, string? availability = null, decimal? rating = null, string? sortBy = null, string? sortOrder = null)
        {
            if (id == null) throw new ArgumentNullException(nameof(id), "Artisan ID cannot be null");

            var products = await _productRepository.GetByArtisanIdAsync(id.Value);

            products = products.Where(p => !p.IsDeleted); // Exclude soft-deleted products // only admin can see soft-deleted products and restore them

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p =>
                    p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Category != null && p.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    p.Status.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    p.Availability.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category!.Name.Equals(category, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(status))
            {
                products = products.Where(p => p.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(availability))
            {
                products = products.Where(p => p.Availability.ToString().Equals(availability, StringComparison.OrdinalIgnoreCase));
            }
            if (rating.HasValue)
            {
                products = products.Where(p => p.Rating.HasValue && p.Rating.Value >= rating.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                products = sortBy.ToLower() switch
                {
                    "name" => sortOrder == "desc" ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                    "price" => sortOrder == "desc" ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                    "category" => sortOrder == "desc" ? products.OrderByDescending(p => p.Category!.Name) : products.OrderBy(p => p.Category!.Name),
                    "status" => sortOrder == "desc" ? products.OrderByDescending(p => p.Status) : products.OrderBy(p => p.Status),
                    "quantityLeft" => sortOrder == "desc" ? products.OrderByDescending(p => p.QuantityLeft) : products.OrderBy(p => p.QuantityLeft),
                    "rating" => sortOrder == "desc" ? products.OrderByDescending(p => p.Rating) : products.OrderBy(p => p.Rating),
                    "availability" => sortOrder == "desc" ? products.OrderByDescending(p => p.Availability) : products.OrderBy(p => p.Availability),
                    _ => products
                };
            }

            return products;
        }

        public async Task<IEnumerable<ProductCategory>> GetAllCategoriesAsync()
        {
            var categories = await _productRepository.GetAllCategoriesAsync();
            return categories;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            return product;
        }

        public async Task<byte[]?> GetProductImageAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null || string.IsNullOrEmpty(product.ImageUrl))
            {
                return null;
            }

            var imagePath = Path.Combine(_env.WebRootPath, product.ImageUrl);
            if (!File.Exists(imagePath))
            {
                return null;
            }

            return await File.ReadAllBytesAsync(imagePath);
        }

        public async Task<Product> UpdateProductAsync(Guid ArtisanId, Guid ProductId, ArtisanUpdateProductRequest request)
        {
            var product = await _productRepository.GetByIdAsync(ProductId);
            if (product == null)
            {
                throw new Exception("Product not found");
            }

            if (product.ArtisanId != ArtisanId)
            {
                throw new Exception("You are not authorized to update this product");
            }

            if (request.Image is not null && request.Image.Length != 0)
            {
                // Generate a unique name for the new image
                var uniqueImageName = Guid.NewGuid().ToString() + "." + request.ImageExtension;

                var webRootPath = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (string.IsNullOrEmpty(webRootPath))
                {
                    throw new InvalidOperationException("WebRootPath is not configured. Ensure wwwroot exists or is properly set up.");
                }

                if (!Directory.Exists(webRootPath)) Directory.CreateDirectory(webRootPath);

                // Save the new image to the file system
                var imageFolder = Path.Combine(_env.WebRootPath!, "images");
                var newImagePath = Path.Combine(imageFolder, uniqueImageName);

                // Ensure the directory exists
                if (!Directory.Exists(imageFolder)) Directory.CreateDirectory(imageFolder);

                await File.WriteAllBytesAsync(newImagePath, Convert.FromBase64String(request.Image));

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath!, product.ImageUrl);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }

                // Update the product's image URL
                product.ImageUrl = Path.Combine("images", uniqueImageName);
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.QuantityLeft = request.QuantityLeft;
            product.Availability = request.Availability == "available" ? ProductAvailability.Available : ProductAvailability.Unavailable;
            product.Status = request.QuantityLeft > 0 ? ProductStatus.InStock : ProductStatus.OutOfStock;
            product.CategoryId = await _productRepository.DoesCategoryExistAsync(request.Category);
            product.UpdatedAt = DateTime.UtcNow;

            await _productRepository.UpdateAsync(product);
            return product;
        }
        #endregion

        #region Orders

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(Guid ArtisanId, string? status = null, int? year = null, string? sortBy = null, string? sortOrder = null)
        {
            var orders = await _orderRepository.GetOrdersByArtisanIdAsync(ArtisanId);

            if (!string.IsNullOrEmpty(status))
            {
                orders = orders.Where(o => o.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (year.HasValue)
            {
                orders = orders.Where(o => o.CreatedAt.Year == year.Value).ToList();
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

            return orders;
        }
        public async Task<Order> GetOrderByIdAsync(Guid ArtisanId, Guid id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null || order.ArtisanId != ArtisanId)
            {
                throw new Exception("Order not found or you are not authorized to view this order");
            }

            return order;
        }

        public async Task<Order> UpdateOrderStatusAsync(Guid ArtisanId, Guid id, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null || order.ArtisanId != ArtisanId)
            {
                throw new Exception("Order not found or you are not authorized to update this order");
            }

            if (!Enum.TryParse(status, true, out OrderStatus orderStatus))
            {
                throw new ArgumentException("Invalid order status", nameof(status));
            }

            if (status == "Pending" || status == "Cancelled" || status == "Delivered")
            {
                throw new InvalidOperationException("Cannot update order status to Pending, Cancelled, or Delivered directly.");
            }

            order.Status = orderStatus;
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateOrderAsync(order);

            // Create a delivery status update
            var deliveryStatusUpdate = new DeliveryStatusUpdate
            {
                OrderId = order.Id,
                Status = orderStatus == OrderStatus.Shipped ? DeliveryStatus.InTransit : DeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
            await _orderRepository.CreateDeliveryStatusUpdateAsync(deliveryStatusUpdate);

            await _orderRepository.SaveChangesAsync();

            return order;
        }

        public async Task RespondToReviewAsync(Guid ArsitsanId, Guid reviewId, ArtisanRespondToReviewRequest request)
        {
            var review = await _productRepository.GetReviewByIdAsync(reviewId);
            if (review == null || review.Product.ArtisanId != ArsitsanId)
            {
                throw new Exception("Review not found or you are not authorized to respond to this review");
            }

            if (string.IsNullOrWhiteSpace(request.Response))
            {
                throw new ArgumentException("Response cannot be empty", nameof(request.Response));
            }

            if (!string.IsNullOrWhiteSpace(review.ArtisanResponse))
            {
                throw new InvalidOperationException("Response already exists for this review.");
            }

            review.ArtisanResponse = request.Response;
            review.UpdatedAt = DateTime.UtcNow;
            await _productRepository.UpdateReviewAsync(review);
        }

        public async Task<ArtisanProfileResponse?> GetArtisanAsync(Guid artisanId)
        {
            var user = await userRepository.GetUserByIdAsync(artisanId);
            if (user is null || user.ArtisanProfile is null)
            {
                return null; // TODO handle user not found or customer profile not found
            }

            return new ArtisanProfileResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Bio = user.ArtisanProfile.Bio,
                City = user.ArtisanProfile.City,
            };
        }

        public async Task<bool> CheckEmailExistsAsync(string email) => await userRepository.GetUserByEmailAsync(email) is not null;
        public async Task<bool> CheckUsernameExistsAsync(string username) => await userRepository.GetUserByUsernameAsync(username) is not null;

        public async Task<bool> UpdateArtisanAsync(Guid artisanId, ArtisanUpdateProfileRequest request)
        {
            var user = await userRepository.GetUserByIdAsync(artisanId);
            if (user is null || user.ArtisanProfile is null)
            {
                throw new Exception("Artisan not found");
            }

            if (await userRepository.GetUserByEmailAsync(request.Email!) is not null && user.Email != request.Email)
            {
                throw new Exception("Email already exists");
            }

            if (await userRepository.GetUserByUsernameAsync(request.Username!) is not null && user.Username != request.Username)
            {
                throw new Exception("Username already exists");
            }

            user.ArtisanProfile.Bio = request.Bio!;
            user.ArtisanProfile.City = request.City!;
            user.Username = request.Username!;
            user.Email = request.Email!;
            user.UpdatedAt = DateTime.UtcNow;

            await userRepository.UpdateUserAsync(user);

            return true; // TODO handle user not found or customer profile not found
        }

        #endregion
    }
}