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
    public class AdminService(
        IUserRepository userRepository,
        IProductRepository productRepository
    ) : IAdminService
    {
        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            user.Status = user.Status == UserStatus.Active ? UserStatus.Inactive : UserStatus.Active;
            user.UpdatedAt = DateTime.UtcNow;

            if (user.Status == UserStatus.Inactive)
            {
                var tokens = await userRepository.GetRefreshTokensByUserIdAsync(userId);
                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }
            }


            await userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var product = await productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return false; // Product not found
            }

            product.IsDeleted = product.IsDeleted ? false : true;
            product.DeletedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            await productRepository.UpdateAsync(product);
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false; // User not found
            }

            user.IsDeleted = user.IsDeleted ? false : true;
            user.DeletedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            if (user.IsDeleted)
            {
                user.Status = UserStatus.Inactive;
                var tokens = await userRepository.GetRefreshTokensByUserIdAsync(user.Id);
                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }
            }

            await userRepository.UpdateUserAsync(user);
            return true;
        }

        public async Task<IEnumerable<ProductResponse>> GetAllProductsAsync(string? search = null, string? artisan = null, string? category = null, string? sortBy = null, string? sortOrder = null)
        {
            var products = await productRepository.GetAllAsync();

            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(p => p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                p.Description.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                p.Artisan.User.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                (p.Category != null && p.Category.Name.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(artisan))
            {
                products = products.Where(p => p.Artisan.User.Username.Equals(artisan, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category != null && p.Category.Name.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                products = sortBy.ToLower() switch
                {
                    "name" => sortOrder?.ToLower() == "desc" ? products.OrderByDescending(p => p.Name) : products.OrderBy(p => p.Name),
                    "price" => sortOrder?.ToLower() == "desc" ? products.OrderByDescending(p => p.Price) : products.OrderBy(p => p.Price),
                    "rating" => sortOrder?.ToLower() == "desc" ? products.OrderByDescending(p => p.Rating) : products.OrderBy(p => p.Rating),
                    "createdat" => sortOrder?.ToLower() == "desc" ? products.OrderByDescending(p => p.CreatedAt) : products.OrderBy(p => p.CreatedAt),
                    "updatedat" => sortOrder?.ToLower() == "desc" ? products.OrderByDescending(p => p.UpdatedAt) : products.OrderBy(p => p.UpdatedAt),
                    _ => products
                };
            }

            return products.Select(p => new ProductResponse
            {
                Id = p.Id,
                ArtisanId = p.ArtisanId,
                ArtisanName = p.Artisan?.User?.Username ?? "Unknown Artisan",
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                QuantityLeft = p.QuantityLeft,
                Category = p.Category?.Name ?? "Uncategorized",
                Availability = p.Availability.ToString(),
                Rating = p.Rating ?? 0,
                ImageUrl = p.ImageUrl,
                ProductReviews = p.ProductReviews.ToList(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
            });
        }

        public async Task<IEnumerable<UserFullProfileResponse>> GetAllUsersAsync(
                string? search = null,
                string? status = null,
                string? role = null,
                bool? isDeleted = null,
                string? sortBy = null,
                string? sortOrder = null
        )
        {
            var users = await userRepository.GetAllUsersAsync();

            users = users.Where(u => u.Role != UserRole.Admin); // Exclude Admins from the list

            if (!string.IsNullOrEmpty(search))
            {
                users = users.Where(u =>
                                u.Username.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                u.Email.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(status))
            {
                users = users.Where(u => u.Status.ToString().Equals(status, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(role))
            {
                users = users.Where(u => u.Role.ToString().Equals(role, StringComparison.OrdinalIgnoreCase));
            }

            if (isDeleted.HasValue)
            {
                users = users.Where(u => u.IsDeleted == isDeleted.Value);
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                users = sortBy.ToLower() switch
                {
                    "username" => sortOrder?.ToLower() == "desc" ? users.OrderByDescending(u => u.Username) : users.OrderBy(u => u.Username),
                    "email" => sortOrder?.ToLower() == "desc" ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
                    "createdat" => sortOrder?.ToLower() == "desc" ? users.OrderByDescending(u => u.CreatedAt) : users.OrderBy(u => u.CreatedAt),
                    "updatedat" => sortOrder?.ToLower() == "desc" ? users.OrderByDescending(u => u.UpdatedAt) : users.OrderBy(u => u.UpdatedAt),
                    _ => users
                };
            }

            return users.Select(u => new UserFullProfileResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Status = u.Status.ToString(),
                Role = u.Role.ToString(),
                IsDeleted = u.IsDeleted,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                DeletedAt = u.DeletedAt,
                ShippingAddress = u.CustomerProfile?.ShippingAddress,
                Bio = u.ArtisanProfile?.Bio,
                City = u.ArtisanProfile?.City,
            });
        }
    }
}