namespace EmailingService.Contracts
{
    public interface IPrettyEmail : IEmailService
    {
        Task SendEmailVerification(string to, string link);
        Task SendPasswordReset(string to, string link, string token);
        Task SendEmailChange(string to, string link);
        Task Send2FAToken(string to, string token);
    }
}
