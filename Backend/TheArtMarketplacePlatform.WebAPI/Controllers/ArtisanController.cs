using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    public class CheckArtisanIdAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var artisanId = context.RouteData.Values["artisanId"] as string;
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
    [Authorize(Roles = "Artisan")]
    [CheckArtisanId]
    [Route("api/artisans/{artisanId:guid}")]
    public class ArtisanController(IArtisanService _artisanService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetArtisanProfile([FromRoute] Guid artisanId)
        {
            var artisanProfile = await _artisanService.GetArtisanAsync(artisanId);
            if (artisanProfile == null)
            {
                return NotFound();
            }
            return Ok(artisanProfile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateArtisanProfile([FromRoute] Guid artisanId, [FromBody] ArtisanUpdateProfileRequest request)
        {
            var updatedProfile = await _artisanService.UpdateArtisanAsync(artisanId, request);
            return updatedProfile ? Ok() : NotFound();
        }

        #region Products

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromRoute] Guid artisanId,
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] string? status = null,
            [FromQuery] string? availability = null,
            [FromQuery] decimal? rating = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var products = await _artisanService.GetAllProductsAsync(artisanId, search, category, status, availability, rating, sortBy, sortOrder);
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _artisanService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpGet("products/{id}/image")]
        public async Task<IActionResult> GetProductImage(Guid id)
        {
            var image = await _artisanService.GetProductImageAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image, "image/jpeg");
        }

        [HttpGet("/api/artisans/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _artisanService.GetAllCategoriesAsync();
            return Ok(categories);
        }


        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromRoute] Guid artisanId, [FromBody] ArtisanInsertProductRequest request)
        {
            var createdProduct = await _artisanService.CreateProductAsync(artisanId, request);
            return Created();
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid artisanId, Guid id, [FromBody] ArtisanUpdateProductRequest request)
        {
            var updatedProduct = await _artisanService.UpdateProductAsync(artisanId, id, request);
            if (updatedProduct == null)
            {
                return NotFound();
            }
            return Ok(updatedProduct);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid artisanId, Guid id)
        {
            var result = await _artisanService.DeleteProductAsync(artisanId, id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        #endregion

        #region Orders

        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromRoute] Guid artisanId,
            [FromQuery] string? status = null,
            [FromQuery] int? year = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var orders = await _artisanService.GetAllOrdersAsync(artisanId, status, year, sortBy, sortOrder);
            return Ok(orders);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderById(Guid artisanId, Guid id)
        {
            var order = await _artisanService.GetOrderByIdAsync(artisanId, id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(Guid artisanId, Guid id, [FromBody] ArtisanUpdateOrderStatusRequest request)
        {
            var updatedOrder = await _artisanService.UpdateOrderStatusAsync(artisanId, id, request.Status);
            if (updatedOrder == null)
            {
                return NotFound();
            }
            return Ok(updatedOrder);
        }

        [HttpPost("reviews/{reviewId}/response")]
        public async Task<IActionResult> RespondToReview(Guid artisanId, Guid reviewId, [FromBody] ArtisanRespondToReviewRequest request)
        {
            await _artisanService.RespondToReviewAsync(artisanId, reviewId, request);
            return Ok();
        }

        #endregion
    }
}