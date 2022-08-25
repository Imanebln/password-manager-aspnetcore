using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailingService.Contracts
{
    public interface IPrettyEmail: IEmailService
    {
        void SendEmailVerification(string to, string link);
        void SendPasswordReset(string to, string link, string token);
        void SendEmailChange(string to, string link);
    }
}
