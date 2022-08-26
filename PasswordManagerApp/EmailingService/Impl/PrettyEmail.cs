using Data.Models.Email;
using EmailingService.Contracts;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailingService.Impl
{
    public class PrettyEmail: EmailService, IPrettyEmail
    {
        private readonly IConfiguration _config;
        public PrettyEmail(EmailConfiguration emailConfiguration): base(emailConfiguration)
        {
            _config = new ConfigurationBuilder().AddJsonFile(@"resources\DefaultEmail.json").Build();
        }

        protected override MimeMessage CreateEmailMessage(Email email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfiguration.DisplayName, _emailConfiguration.From));
            emailMessage.To.Add(MailboxAddress.Parse(email.To));
            emailMessage.Subject = email.Subject;
            BodyBuilder builder = new();
            builder.HtmlBody = EmailParser.GetStyledBody(email.EmailBody);
            emailMessage.Body = builder.ToMessageBody();
            return emailMessage;
        }
        public async Task SendEmailVerification(string to, string link)
        {
            Email email = GetEmail("VALIDATION", link);
            email.To = to;
            await SendEmailAsync(email);
        }
        private Email GetEmail(string EmailName,string? link)
        {
            Email email = new()
            {
                Subject = getString(EmailName,"Subject")
            };
            email.EmailBody.Title = getString(EmailName, "Title");
            email.EmailBody.Message = getString(EmailName, "Message");
            email.EmailBody.Type = (EmailType)Enum.Parse(typeof(EmailType), getString(EmailName, "Type"), true);

            if (link != null)
            {
                email.EmailBody.Buttons.Add(new Button()
                {
                    Link = link,
                    Text = getString(EmailName, "ButtonText")
                });
            }

            return email;

            string getString(string name, string local)
            {
                return _config[$"{name}:{local}"];
            }
        }

        public async Task SendPasswordReset(string to, string link,string token)
        {
            Email email = GetEmail("PASSWORD_RESET", link);
            email.To = to;
            email.EmailBody.Message += "\n"+token;
            await SendEmailAsync(email);
        }
        public async Task SendEmailChange(string to, string link)
        {
            Email email = GetEmail("EMAIL_CHANGE", link);
            email.To = to;
            await SendEmailAsync(email);
        }
    }
}
