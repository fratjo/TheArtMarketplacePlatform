using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TheArtMarketplacePlatform.BusinessLayer.Exceptions;
using TheArtMarketplacePlatform.BusinessLayer.Validators;
using TheArtMarketplacePlatform.Core.DTOs;
using TheArtMarketplacePlatform.Core.Entities;
using TheArtMarketplacePlatform.Core.Interfaces.Services;
using TheArtMarketplacePlatform.Core.Interfaces.Repositories;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class AuthService(IUserRepository _userRepository, IConfiguration _config) : IAuthService
    {
        public async Task<bool> CheckEmailExistsAsync(string email) => await _userRepository.GetUserByEmailAsync(email) is not null;
        public async Task<bool> CheckUsernameExistsAsync(string username) => await _userRepository.GetUserByUsernameAsync(username) is not null;
        public async Task<string> RegisterArtisanAsync(RegisterArtisanRequest request)
        {
            // Check if the email is already in use
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null) throw new ArgumentException("Email is already in use.");

            // Generate a salt and hash the password
            var salt = GenerateSalt();
            var hashedPassword = HashPasswordWithSalt(request.Password, salt);

            // Store the user
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Role = UserRole.Artisan,
            };

            user.ArtisanProfile = new ArtisanProfile
            {
                // Set artisan-specific properties here
                UserId = user.Id,
                Bio = request.Bio,
                City = request.City,
            };

            // Save the user to the database
            await _userRepository.SaveUserAsync(user);

            // Check if the user was saved successfully
            if (user.Id == Guid.Empty) throw new Exception("Failed to register new Artisan.");

            // Generate a JWT token
            var token = GenerateToken(user);

            // Return the token
            return token;
        }

        public async Task<string> RegisterCustomerAsync(RegisterCustomerRequest request)
        {
            // Check if the email is already in use
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null) throw new ArgumentException("Email is already in use.");

            // Generate a salt and hash the password
            var salt = GenerateSalt();
            var hashedPassword = HashPasswordWithSalt(request.Password, salt);

            // Store the user
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Role = UserRole.Customer,
            };

            user.CustomerProfile = new CustomerProfile
            {
                // Set customer-specific properties here
                UserId = user.Id,
                ShippingAddress = request.ShippingAddress,
            };

            // Save the user to the database
            await _userRepository.SaveUserAsync(user);

            // Check if the user was saved successfully
            if (user.Id == Guid.Empty) throw new Exception("Failed to register new Customer.");

            // Generate a JWT token
            var token = GenerateToken(user);

            // Return the token
            return token;
        }

        public async Task<string> RegisterDeliveryPartnerAsync(RegisterDeliveryPartnerRequest request)
        {
            // Check if the email is already in use
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null) throw new ArgumentException("Email is already in use.");

            // Generate a salt and hash the password
            var salt = GenerateSalt();
            var hashedPassword = HashPasswordWithSalt(request.Password, salt);

            // Store the user
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim().ToLower(),
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Role = UserRole.DeliveryPartner,
            };

            user.DeliveryPartnerProfile = new DeliveryPartnerProfile
            {
                // Set delivery partner-specific properties here
                UserId = user.Id,
            };

            // Save the user to the database
            await _userRepository.SaveUserAsync(user);

            // Check if the user was saved successfully
            if (user.Id == Guid.Empty) throw new Exception("Failed to register new DeliveryPartner.");

            // Generate a JWT token
            var token = GenerateToken(user);

            // Return the token
            return token;
        }

        public async Task<string> LoginUserAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            // Check if the user exists and is active
            if (user is null) throw new InvalidCredentialsException("Invalid email or password.");

            // Verify the password
            if (!VerifyPassword(request.Password, user.PasswordSalt, user.PasswordHash)) throw new InvalidCredentialsException("Invalid email or password.");

            // Verify if the user is active
            if (user.Status != UserStatus.Active) throw new InactiveAccountException("Account is inactive.");

            // Check if account is banned 
            if (user.Status == UserStatus.Banned) throw new InactiveAccountException("Account is banned.");

            // Check if the user is deleted
            if (user.IsDeleted) throw new InactiveAccountException("Account is deleted.");

            // Generate a JWT token
            var token = GenerateToken(user);

            // Return the token
            return token;
        }


        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateSalt()
        {
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[16];
                rng.GetBytes(saltBytes);
                return Convert.ToBase64String(saltBytes);
            }
        }

        private string HashPasswordWithSalt(string password, string salt)
        {
            using (var sha = System.Security.Cryptography.SHA512.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToHexString(hash);
            }
        }

        private bool VerifyPassword(string password, string salt, string hashedPassword)
        {
            var hash = HashPasswordWithSalt(password, salt);
            return hash == hashedPassword;
        }
    }
}