using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class GuestController(IGuestService guestService) : ControllerBase
    {
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] string? status = null,
            [FromQuery] string? availability = null,
            [FromQuery] decimal? rating = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var products = await guestService.GetAllProductsAsync(search, category, status, availability, rating, sortBy, sortOrder);
            if (products == null || !products.Any()) return NotFound();
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await guestService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}