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
    public class ArtisanController(IProductService _productService) : ControllerBase
    {
        #region Products

        [HttpGet("products")]
        public async Task<IActionResult> GetProducts([FromRoute] Guid artisanId)
        {
            var products = await _productService.GetAllProductsAsync(artisanId);
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromRoute] Guid artisanId, [FromBody] ArtisanInsertProductRequest request)
        {
            var createdProduct = await _productService.CreateProductAsync(artisanId, request);
            return Created();
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid artisanId, [FromBody] ArtisanUpdateProductRequest request)
        {
            var updatedProduct = await _productService.UpdateProductAsync(artisanId, request);
            if (updatedProduct == null)
            {
                return NotFound();
            }
            return Ok(updatedProduct);
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] Guid artisanId, Guid id)
        {
            var result = await _productService.DeleteProductAsync(artisanId, id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        #endregion
    }
}