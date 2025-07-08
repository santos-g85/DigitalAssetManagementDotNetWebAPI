using DAMApi.DTOs;
using DAMApi.Models.Entities;

namespace DAMApi.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<UserModel> AddUserAsync(UserModel user);
        Task<UserModel> UpdateUserAsync(UserModel user);
        Task<UserModel> DeleteUserAsync(string id);
        Task<UserModel> GetUsersByIdAsync(string id);

        Task<UserModel> GetUsersByEmailIdAsync(string email);
        Task<List<UserModel>> GetAllUsersAsync();

    }
}
