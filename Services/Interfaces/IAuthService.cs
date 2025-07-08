using DAMApi.DTOs;
using DAMApi.Models.DTOs;
using DAMApi.Models.Entities;

namespace DAMApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResult<UserModel?>> RegisterAsync(UserRegisterDto request);
        Task<ServiceResult<TokenResponseDto?>> LoginAsync(UserLoginDto request);

    }
}
