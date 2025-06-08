using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController(IAdminService adminService) : ControllerBase
    {
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] string? search = null,
            [FromQuery] string? status = null,
            [FromQuery] bool? isDeleted = null,
            [FromQuery] string? role = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var users = await adminService.GetAllUsersAsync(search, status, role, isDeleted, sortBy, sortOrder);
            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? search = null,
            [FromQuery] string? artisan = null,
            [FromQuery] string? category = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var products = await adminService.GetAllProductsAsync(search, artisan, category, sortBy, sortOrder);
            return Ok(products ?? new List<ProductResponse>());
        }

        [HttpDelete("users/{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await adminService.DeleteUserAsync(userId);
            if (result)
            {
                return NoContent(); // 204 No Content
            }
            return NotFound("User not found.");
        }

        [HttpDelete("users/{userId:guid}/deactivate")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            var result = await adminService.DeactivateUserAsync(userId);
            if (result)
            {
                return NoContent(); // 204 No Content
            }
            return NotFound("User not found.");
        }

        [HttpDelete("products/{productId:guid}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            var result = await adminService.DeleteProductAsync(productId);
            if (result)
            {
                return NoContent(); // 204 No Content
            }
            return NotFound("Product not found.");
        }
    }
}