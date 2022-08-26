using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailingService.Contracts
{
    public interface IPrettyEmail: IEmailService
    {
        Task SendEmailVerification(string to, string link);
        Task SendPasswordReset(string to, string link, string token);
        Task SendEmailChange(string to, string link);
    }
}
