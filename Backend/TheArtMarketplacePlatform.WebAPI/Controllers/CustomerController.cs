using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    public class CheckCustomerIdAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var artisanId = context.RouteData.Values["customerId"] as string;
            if (artisanId == null || !Guid.TryParse(artisanId, out var id))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || Guid.Parse(userId) != id)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }

    [ApiController]
    [Authorize(Roles = "Customer")]
    [CheckCustomerId]
    [Route("api/customers/{customerId:guid}")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        [HttpGet()]
        public async Task<IActionResult> GetCustomer(Guid customerId)
        {
            var customer = await customerService.GetCustomerAsync(customerId);
            return customer != null ? Ok(customer) : NotFound("Customer not found");
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] CustomerUpdateProfileRequest request)
        {
            var updated = await customerService.UpdateCustomerAsync(customerId, request);
            return updated ? Ok() : NotFound("Customer not found or update failed");
        }

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CustomerInsertOrderRequest orderDto)
        {
            var result = await customerService.CreateOrderAsync(orderDto.CustomerId, orderDto.DeliveryPartnerId, orderDto.OrderProducts);
            return result ? Ok() : BadRequest("Failed to create order");
        }
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders(Guid customerId)
        {
            var orders = await customerService.GetOrdersAsync(customerId);
            return Ok(orders);
        }

        [HttpGet("orders/{orderId}")]
        public async Task<IActionResult> GetOrder(Guid customerId, Guid orderId)
        {
            var order = await customerService.GetOrderAsync(customerId, orderId);
            return order != null ? Ok(order) : NotFound("Order not found");
        }

        [HttpGet("already-bought-reviewed/{productId:guid}")]
        public async Task<IActionResult> AlreadyBoughtReviewed(Guid customerId, Guid productId)
        {
            var result = await customerService.AlreadyBoughtReviewedAsync(customerId, productId);
            return Ok(result);
        }

        [HttpPost("review-product")]
        public async Task<IActionResult> ReviewProduct(Guid customerId, [FromBody] CustomerLeaveProductReviewRequest review)
        {
            var result = await customerService.ReviewProductAsync(customerId, review);
            return result ? Ok() : BadRequest("Failed to review product");
        }

        [HttpGet("products/favorites")]
        public async Task<IActionResult> GetFavoriteProducts(Guid customerId)
        {
            var products = await customerService.GetFavoriteProductsAsync(customerId);
            return Ok(products);
        }

        [HttpPost("products/favorites/{productId:guid}")]
        public async Task<IActionResult> AddProductToFavorites(Guid customerId, Guid productId)
        {
            var result = await customerService.AddProductToFavoritesAsync(customerId, productId);
            return result ? Ok() : BadRequest("Failed to add product to favorites");
        }

        [HttpDelete("products/favorites/{productId:guid}")]
        public async Task<IActionResult> RemoveProductFromFavorites(Guid customerId, Guid productId)
        {
            var result = await customerService.RemoveProductFromFavoritesAsync(customerId, productId);
            return result ? Ok() : BadRequest("Failed to remove product from favorites");
        }
    }
}