using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.Interfaces;
using TheArtMarketplacePlatform.Core.DTOs;


namespace TheArtMarketplacePlatform.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        // api/auth/email
        [HttpGet("email")]
        public async Task<IActionResult> CheckEmailExists([FromQuery] string email)
        {
            var exists = await _authService.CheckEmailExistsAsync(email);
            return Ok(new { Exists = exists });
        }

        // api/auth/username
        [HttpGet("username")]
        public async Task<IActionResult> CheckUsernameExists([FromQuery] string username)
        {
            var exists = await _authService.CheckUsernameExistsAsync(username);
            return Ok(new { Exists = exists });
        }

        // api/auth/register/artisan
        [HttpPost("register/artisan")]
        public async Task<IActionResult> RegisterArtisan([FromBody] RegisterArtisanRequest request)
        {
            var token = await _authService.RegisterArtisanAsync(request);
            return Ok(new { Token = token });
        }

        // api/auth/register/customer
        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request)
        {
            var token = await _authService.RegisterCustomerAsync(request);
            return Ok(new { Token = token });
        }

        // api/auth/register/delivery-partner
        [HttpPost("register/delivery-partner")]
        public async Task<IActionResult> RegisterDeliveryPartner([FromBody] RegisterDeliveryPartnerRequest request)
        {
            var token = await _authService.RegisterDeliveryPartnerAsync(request);
            return Ok(new { Token = token });
        }

        // api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginUserAsync(request);
            return Ok(new { Token = token });
        }
    }
}