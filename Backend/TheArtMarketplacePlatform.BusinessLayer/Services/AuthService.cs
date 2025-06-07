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
using System.Security.Cryptography;

namespace TheArtMarketplacePlatform.BusinessLayer.Services
{
    public class AuthService(IUserRepository _userRepository, IConfiguration _config) : IAuthService
    {
        public async Task<bool> CheckEmailExistsAsync(string email) => await _userRepository.GetUserByEmailAsync(email) is not null;
        public async Task<bool> CheckUsernameExistsAsync(string username) => await _userRepository.GetUserByUsernameAsync(username) is not null;
        public async Task<AuthResponse> RegisterArtisanAsync(RegisterArtisanRequest request)
        {
            // Check if the email is already in use
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null) throw new ArgumentException("Email is already in use.");

            // Check if the username is already in use
            var existingUsername = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (existingUsername != null) throw new ArgumentException("Username is already in use.");

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

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> RegisterCustomerAsync(RegisterCustomerRequest request)
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

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> RegisterDeliveryPartnerAsync(RegisterDeliveryPartnerRequest request)
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

            return await GenerateAuthResponse(user);
        }

        public async Task<AuthResponse> LoginUserAsync(LoginRequest request)
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

            return await GenerateAuthResponse(user);
        }

        private async Task<AuthResponse> GenerateAuthResponse(User user)
        {
            // Generate a JWT token
            var token = GenerateToken(user, DateTime.UtcNow.AddMinutes(1));
            var expirationDate = DateTime.UtcNow.AddHours(24 * 7);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var existingRefreshToken = await _userRepository.GetRefreshTokensByUserIdAsync(user.Id);
            if (existingRefreshToken.Any())
            {
                // Revoke existing refresh tokens
                foreach (var rt in existingRefreshToken)
                {
                    rt.IsRevoked = true;
                }
            }

            var newToken = new Core.Entities.RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = expirationDate
            };

            await _userRepository.SaveRefreshTokenAsync(newToken);

            // Return the token
            return new AuthResponse
            {
                Token = token,
                RefreshToken = refreshToken
            };
        }


        private string GenerateToken(User user, DateTime expirationDate)
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
                expires: expirationDate,
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

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            // Get the user by ID
            var user = _userRepository.GetUserByIdAsync(userId).Result;
            if (user is null) throw new Exception("User not found.");

            // Verify the old password
            if (!VerifyPassword(request.CurrentPassword, user.PasswordSalt, user.PasswordHash))
            {
                throw new InvalidCredentialsException("Current password is incorrect.");
            }

            // Generate a new salt and hash the new password
            var newSalt = GenerateSalt();
            var newHashedPassword = HashPasswordWithSalt(request.NewPassword, newSalt);

            // Update the user's password
            user.PasswordHash = newHashedPassword;
            user.PasswordSalt = newSalt;
            user.UpdatedAt = DateTime.UtcNow;

            // Save the updated user
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<AuthResponse?> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntry = await _userRepository.GetRefreshTokenAsync(refreshToken);
            if (tokenEntry == null || tokenEntry.IsRevoked || tokenEntry.ExpiryDate < DateTime.UtcNow)
                return null;

            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (user is null) return null;

            // Generate a new JWT token and refresh token
            return await GenerateAuthResponse(user);
        }

        public async Task<bool> LogoutUserAsync(string refreshToken)
        {
            // Check if the user exists
            var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
            if (user is null) throw new NotFoundException("User not found.");

            // Check if the refresh token is valid
            var existingTokens = await _userRepository.GetRefreshTokensByUserIdAsync(user.Id);
            var tokenToRevoke = existingTokens.FirstOrDefault(rt => rt.Token == refreshToken && !rt.IsRevoked);
            if (tokenToRevoke is null) throw new NotFoundException("Refresh token not found or already revoked.");

            // Revoke the refresh token
            tokenToRevoke.IsRevoked = true;

            return await _userRepository.UpdateRefreshTokenAsync(tokenToRevoke);
        }
    }
}