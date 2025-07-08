using DAMApi.Models.Entities;

namespace DAMApi.Repository.Interfaces
{
    public interface IFolderRepositroy
    {
        Task<FolderModel> GetFolderByUserId(string UserId);
        Task<FolderModel> CreateFolderRecord(FolderModel folder);
        Task<FolderModel> UpdateFolderRecord(FolderModel folder);
        Task<FolderModel> DeleteFolderRecord(string Id, string UserId);
    }
}
