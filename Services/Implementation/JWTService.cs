using DAMApi.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using DAMApi.Repository.Interfaces;
using DAMApi.Models.DTOs;
using DAMApi.Services.Interfaces;
using System.Threading.Tasks;

namespace DAMApi.Services.Implementation
{
    internal sealed class JWTService : IJWTService
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IFolderRepositroy _folderRepositroy;
        public JWTService(IConfiguration config,
            IUserRepository userRepository,
            IFolderRepositroy folderRepositroy)
        {
            _config = config;
            _userRepository = userRepository;
            _folderRepositroy = folderRepositroy;
        }
        private async Task<string> CreateToken(UserModel user)
        {
            var userId = user.Id;
            var folderName = await _folderRepositroy.GetFolderByUserId(userId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()),
            };

            // siging key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtSettings:JwtKey")!));

            // credentials
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            // token descriptor
            var tokenDescriptor = new JwtSecurityToken(
            issuer: _config.GetValue<string>("JwtSettings:JwtIssuer"),
                audience: _config.GetValue<string>("JwtSettings:JwtAudience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(_config.GetValue<int>("JwtSettings:JwtTokenExpiration")),
                signingCredentials: cred
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
        private async Task<TokenResponseDto> GenerateTokenAsync(UserModel user)
        {
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user), "User cannot be null");
            }
            return await CreateTokenResponseAsync(user);
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        private async Task<string> GenerateAndSaveRefreshTokenAsync(UserModel user)
        {
            var RefreshToken = GenerateRefreshToken();
            user.RefreshToken = RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_config.GetValue<int>("JwtSettings:JwtRefreshTokenExpiration")!);
            await _userRepository.UpdateUserAsync(user);
            return RefreshToken;
        }
        private async Task<UserModel?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _userRepository.GetUsersByIdAsync(userId.ToString());
            if (user is null
                || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }
        public async Task<TokenResponseDto> CreateTokenResponseAsync(UserModel result)
        {
            return new TokenResponseDto
            {
                AccessToken = await CreateToken(result),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(result)
            };
        }
        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user is null)
            {
                return null;
            }
            return await CreateTokenResponseAsync(user);
        }
    }
}
