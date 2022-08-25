using Data.Models;
using Data.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailingService.Contracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(Email email);
        void SendEmail(Email email);
    }
}
