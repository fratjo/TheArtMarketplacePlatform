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
            var result = await customerService.CreateOrderAsync(orderDto.CustomerId, orderDto.OrderProducts);
            return result ? Ok() : BadRequest("Failed to create order");
        }
    }
}