namespace DAMApi.Models.Entities
{
    public class FolderModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? FolderName { get; set; }
        public string? FolderPath { get; set; }
    }
}
