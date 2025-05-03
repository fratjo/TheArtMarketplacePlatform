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
    [Authorize(Roles = "Artisan")]
    [Route("api/artisans/{artisanId:guid}")]
    public class ArtisanController(IArtisanService _artisanService) : ControllerBase
    {
        #region Products

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromRoute] Guid artisanId,
            [FromQuery] string? search = null,
            [FromQuery] string? category = null,
            [FromQuery] string? status = null,
            [FromQuery] string? availability = null,
            [FromQuery] decimal? rating = null)
        {
            var products = await _artisanService.GetAllProductsAsync(artisanId, search, category, status, availability, rating);
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
    }
}