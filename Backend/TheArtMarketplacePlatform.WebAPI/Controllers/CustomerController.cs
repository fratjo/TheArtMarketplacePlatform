using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [HttpPost("orders")]
        public async Task<IActionResult> CreateOrder([FromBody] CustomerInsertOrderRequest orderDto)
        {
            var result = await customerService.CreateOrderAsync(orderDto.CustomerId, orderDto.DeliveryPartnerId, orderDto.OrderProducts);
            return result ? Ok() : BadRequest("Failed to create order");
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
            var order = await customerService.GetOrderAsync(customerId, orderId);
            return order != null ? Ok(order) : NotFound("Order not found");
        }

        [HttpGet("{customerId:guid}/already-bought-reviewed/{productId:guid}")]
        public async Task<IActionResult> AlreadyBoughtReviewed(Guid customerId, Guid productId)
        {
            var result = await customerService.AlreadyBoughtReviewedAsync(customerId, productId);
            return Ok(result);
        }

        [HttpPost("{customerId:guid}/review-product")]
        public async Task<IActionResult> ReviewProduct(Guid customerId, [FromBody] CustomerLeaveProductReviewRequest review)
        {
            var result = await customerService.ReviewProductAsync(customerId, review);
            return result ? Ok() : BadRequest("Failed to review product");
        }
    }
}