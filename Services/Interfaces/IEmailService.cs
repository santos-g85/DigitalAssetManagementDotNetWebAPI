using DAMApi.Models.DTOs;
namespace DAMApi.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailReceiverDto request);

    }
}
