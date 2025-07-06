using DAMApi.Models.DTOs;
using DAMApi.Models.Entities;

namespace DAMApi.Services.Interfaces
{
    public interface IJWTService
    {
        Task<TokenResponseDto> CreateTokenResponseAsync(UserModel result);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}