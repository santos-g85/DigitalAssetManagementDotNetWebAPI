using DAMApi.DTOs;
using DAMApi.Models.DTOs;
using DAMApi.Models.Entities;

namespace DAMApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserModel?> RegisterAsync(UserRegisterDto request);
        Task<TokenResponseDto?> LoginAsync(UserLoginDto request);

    }
}
