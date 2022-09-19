using Data.Models.Email;

namespace EmailingService.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
        void SendEmail(Email email);
    }
}
