using DAMApi.Models.Entities;

namespace DAMApi.Models.DTOs
{
    public class EmailReceiverDto
    {
        public List<string> To { get; set; } = new List<string>();
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
