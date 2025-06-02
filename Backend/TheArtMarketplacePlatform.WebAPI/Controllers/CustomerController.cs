using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "Customer")]
    [Route("api/customers")]
    public class CustomerController(ICustomerService customerService) : ControllerBase
    {
        private void CheckUserId(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || Guid.Parse(userId) != id)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
            }
        }

        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CustomerInsertOrderRequest orderDto)
        {
            var result = await customerService.CreateOrderAsync(orderDto.CustomerId, orderDto.DeliveryPartnerId, orderDto.OrderProducts);
            return result ? Ok() : BadRequest("Failed to create order");
        }

        [HttpGet("{customerId}")]
        public async Task<IActionResult> GetCustomer(Guid customerId)
        {
            CheckUserId(customerId); // TODO handle unauthorized access more gracefully
            var customer = await customerService.GetCustomerAsync(customerId);
            return customer != null ? Ok(customer) : NotFound("Customer not found");
        }

        [HttpPut("{customerId}")]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, [FromBody] CustomerUpdateProfileRequest request)
        {
            CheckUserId(customerId);
            var updated = await customerService.UpdateCustomerAsync(customerId, request);
            return updated ? Ok() : NotFound("Customer not found or update failed");
        }

        [HttpGet("{customerId}/orders")]
        public async Task<IActionResult> GetOrders(Guid customerId)
        {
            var orders = await customerService.GetOrdersAsync(customerId);
            return Ok(orders);
        }

        [HttpGet("{customerId}/orders/{orderId}")]
        public async Task<IActionResult> GetOrder(Guid customerId, Guid orderId)
        {
            CheckUserId(customerId);
            var order = await customerService.GetOrderAsync(customerId, orderId);
            return order != null ? Ok(order) : NotFound("Order not found");
        }

        [HttpGet("{customerId:guid}/already-bought-reviewed/{productId:guid}")]
        public async Task<IActionResult> AlreadyBoughtReviewed(Guid customerId, Guid productId)
        {
            CheckUserId(customerId);
            var result = await customerService.AlreadyBoughtReviewedAsync(customerId, productId);
            return Ok(result);
        }

        [HttpPost("{customerId:guid}/review-product")]
        public async Task<IActionResult> ReviewProduct(Guid customerId, [FromBody] CustomerLeaveProductReviewRequest review)
        {
            CheckUserId(customerId);
            var result = await customerService.ReviewProductAsync(customerId, review);
            return result ? Ok() : BadRequest("Failed to review product");
        }
    }
}