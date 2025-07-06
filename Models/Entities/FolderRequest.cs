using DAMApi.Models.Enums;
using System.Text.Json.Serialization;

namespace DAMApi.Models.Entities
{
    public class FolderRequest
    {
        public string ParentFolderName { get; set; } = string.Empty;
        public List<SubFolderTypes>? SubFolderNames { get; set; } = new();
    }
}
