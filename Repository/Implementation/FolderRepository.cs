using DAMApi.Models.Entities;
using DAMApi.Repository.Interfaces;
using DAMApi.Settings;
using Google.Apis.Drive.v3.Data;
using MongoDB.Driver;

namespace DAMApi.Repository.Implementation
{
    public class FolderRepository : IFolderRepositroy
    {
        private readonly IMongoCollection<FolderModel> _FolderCollection;
        private readonly ILogger<FolderRepository> _Logger;

        public FolderRepository(ApplicationDbContext dbContext,
            ILogger<FolderRepository> logger) 
        { 
            var collectionName = nameof(FolderModel).Replace("Model"," ");
            _FolderCollection = dbContext.GetCollection<FolderModel>(collectionName);
            _Logger = logger;
        }
        public async Task<FolderModel> CreateFolderRecord(FolderModel folder)
        {
            try
            {
                 await _FolderCollection.InsertOneAsync(folder);
                _Logger.LogInformation("Inserted folder record!");
                return folder;
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex.Message);
                return null;
            }
        }

        public Task<FolderModel> DeleteFolderRecord(string Id, string UserId)
        {
            throw new NotImplementedException();
        }

        public async Task<FolderModel> GetFolderByUserId(string UserId)
        {
            try
            {   var filter = Builders<FolderModel>.Filter.Eq(x=>x.UserId,UserId);
                var folders = _FolderCollection.Find(filter).FirstOrDefault();
                await _FolderCollection.InsertOneAsync(folders);
                return folders;
            }
            catch (Exception ex)
            { 
                _Logger.LogError(ex.Message);
                return null;
            }
        }

        public Task<FolderModel> UpdateFolderRecord(FolderModel folder)
        {
            throw new NotImplementedException();
        }
    }
}
