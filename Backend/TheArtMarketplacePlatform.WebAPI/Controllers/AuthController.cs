using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.Core.Interfaces.Services;
using TheArtMarketplacePlatform.Core.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


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
            var tokens = await _authService.RegisterArtisanAsync(request);
            return Ok(tokens);
        }

        // api/auth/register/customer
        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] RegisterCustomerRequest request)
        {
            var tokens = await _authService.RegisterCustomerAsync(request);
            return Ok(tokens);
        }

        // api/auth/register/delivery-partner
        [HttpPost("register/delivery-partner")]
        public async Task<IActionResult> RegisterDeliveryPartner([FromBody] RegisterDeliveryPartnerRequest request)
        {
            var tokens = await _authService.RegisterDeliveryPartnerAsync(request);
            return Ok(tokens);
        }

        // api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var tokens = await _authService.LoginUserAsync(request);
            return Ok(tokens);
        }

        // api/auth/logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshToken refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken.Token))
            {
                return BadRequest(new { Message = "Refresh token is required." });
            }

            var result = await _authService.LogoutUserAsync(refreshToken.Token);
            if (result)
            {
                return Ok(new { Message = "Logged out successfully." });
            }
            return BadRequest(new { Message = "Failed to log out." });
        }

        // api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshToken refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken.Token))
            {
                return BadRequest(new { Message = "Refresh token is required." });
            }

            var tokens = await _authService.RefreshTokenAsync(refreshToken.Token);
            if (tokens == null)
            {
                return Unauthorized(new { Message = "Invalid refresh token." });
            }

            return Ok(tokens);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var userId))
            {
                return Unauthorized(new { Message = "User not authenticated." });
            }

            var result = await _authService.ChangePasswordAsync(Guid.Parse(id), request);

            if (result)
            {
                return Ok(new { Message = "Password changed successfully." });
            }
            return BadRequest(new { Message = "Failed to change password." });
        }
    }
}