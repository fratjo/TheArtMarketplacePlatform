using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Services;

namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    [ApiController]
    [Authorize(Roles = "DeliveryPartner")]
    [Route("api/deliverypartners/{deliveryPartnerId:guid}")]
    public class DeliveryPartnerController(IDeliveryPartnerService deliveryPartnerService) : ControllerBase
    {
        private void CheckUserId(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || Guid.Parse(userId) != id)
            {
                throw new UnauthorizedAccessException("You are not authorized to access this resource.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryPartnerProfile([FromRoute] Guid deliveryPartnerId)
        {
            CheckUserId(deliveryPartnerId);
            var artisanProfile = await deliveryPartnerService.GetDeliveryPartnerAsync(deliveryPartnerId);
            if (artisanProfile == null)
            {
                return NotFound();
            }
            return Ok(artisanProfile);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDeliveryPartnerProfile([FromRoute] Guid deliveryPartnerId, [FromBody] DeliveryPartnerUpdateProfileRequest request)
        {
            CheckUserId(deliveryPartnerId);
            var updatedProfile = await deliveryPartnerService.UpdateDeliveryPartnerAsync(deliveryPartnerId, request);
            return updatedProfile ? Ok() : NotFound();
        }

        [HttpGet("deliveries")]
        public async Task<IActionResult> GetDeliveryOrders(
            [FromRoute] Guid deliveryPartnerId,
            [FromQuery] string? status = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var deliveries = await deliveryPartnerService.GetDeliveryOrdersByDeliveryPartnerAsync(
                deliveryPartnerId, status, sortBy, sortOrder);
            return Ok(deliveries);
        }

        [HttpPost("deliveries/{deliveryId:guid}/set-as-delivered")]
        public async Task<IActionResult> SetDeliveryAsDelivered(
            [FromRoute] Guid deliveryPartnerId,
            [FromRoute] Guid deliveryId)
        {
            var result = await deliveryPartnerService.SetDeliveryAsDeliveredAsync(deliveryPartnerId, deliveryId);
            if (result)
            {
                return NoContent();
            }
            return NotFound(new { Message = "Delivery not found or already delivered." });
        }
    }
}