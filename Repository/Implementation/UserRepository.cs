using DAMApi.DTOs;
using DAMApi.Models.Entities;
using DAMApi.Repository.Interfaces;
using DAMApi.Settings;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;

namespace DAMApi.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<UserModel> _Collection;
        private readonly ILogger<UserRepository> _Logger;
        private readonly PasswordHasher<object> _hasher = new();
        public UserRepository(ApplicationDbContext dbcontext,
            ILogger<UserRepository> logger)
        {
            var collectionName = nameof(UserModel).Replace("Model", "");
            _Collection = dbcontext.GetCollection<UserModel>(collectionName);
            _Logger = logger;
        }

        public async Task<UserModel> AddUserAsync(UserModel user)
        {
            try
            {
                await _Collection.InsertOneAsync(user);
                _Logger.LogInformation($"User with email {user.Email} added successfully.");
                return user;
            }
            catch(Exception ex)
            {
                _Logger.LogError(ex.Message);
                return null;
            }
        }

        public async Task<UserModel> DeleteUserAsync(string id)
        {
            var filter = Builders<UserModel>.Filter.Eq(r => r.Id, id);
            return await _Collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<List<UserModel>> GetAllUsersAsync()
        {
            var users = await _Collection.Find(_ => true).ToListAsync();

            if (users == null || !users.Any())
            {
                _Logger.LogWarning("No receivers found.");
                return new List<UserModel>();
            }
            return users;
        }

        public Task<UserModel> GetUsersByEmailIdAsync(string email)
        {
            var user = _Collection.Find(u => u.Email == email).FirstOrDefaultAsync();

            if (user == null)
            {
                _Logger.LogWarning($"User with email {email} not found.");
                return null;
            }
            return user;
        }

        public async Task<UserModel> GetUsersByIdAsync(string id)
        {
            var filter = Builders<UserModel>.Filter.Eq(r => r.Id, id);
            var user = await _Collection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                _Logger.LogWarning($"Receiver with ID {id} not found.");
                throw new KeyNotFoundException();
            }
            return user;
        }

        public  async Task<UserModel> UpdateUserAsync(UserModel user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "Receiver cannot be null.");
            }

            var filter = Builders<UserModel>.Filter.Eq(r => r.Id, user.Id);

            if(filter is null)
            {
                _Logger.LogWarning("User  not found");
                throw new ArgumentNullException(nameof(filter), "User not found!.");
            }

            var update = Builders<UserModel>.Update
                .Set(r => r.FullName, user.FullName)
                .Set(r => r.PhoneNumber, user.PhoneNumber)
                .Set(r => r.Email, user.Email)
                .Set(r=>r.RefreshToken,user.RefreshToken)
                .Set(r =>r.RefreshTokenExpiryTime,user.RefreshTokenExpiryTime);


            var result = await _Collection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                _Logger.LogWarning($"Receiver with ID {user.Id} not found.");
                return null;
            }

            return user;
        }
    }
}
