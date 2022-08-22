using Data.Models;
using Data.Models.Email;
using EmailingService.Contracts;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;
using System.Web;

namespace EmailingService.Impl
{
    public class EmailService : IEmailService
    {
        protected readonly EmailConfiguration _emailConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        public EmailService(EmailConfiguration emailConfiguration, UserManager<ApplicationUser> userManager)
        {
            _emailConfiguration = emailConfiguration;
            _userManager = userManager;
        }
        public async Task SendEmailAsync(Email email)
        {
            var emailMessage = CreateEmailMessage(email);

            await SendAsync(emailMessage);
        }
        // Email Validation
        public async Task<string> EmailValidation(ApplicationUser user)
        {
            //Generate confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // redirect user to login page
            token = HttpUtility.UrlEncode(token);
            var confirmationLink = "https://localhost:7077/api/Accounts/confirm-email?token=" + token + "&email=" + user.Email;
            
            Email email = new()
            {
                To = user.Email,
                Subject = "Email Confirmation",
                Content = string.Format("<h2 style='color: red;'>Confirm your email, please click this link:</h2>"),
                Link = confirmationLink,
                From = _emailConfiguration.From
            };
            await SendEmailAsync(email);
            return "Success!";
        }

        //Create email message
        protected virtual MimeMessage CreateEmailMessage(Email email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfiguration.From));
            emailMessage.From.Add(new MailboxAddress(_emailConfiguration.DisplayName, _emailConfiguration.From));
            emailMessage.To.Add(MailboxAddress.Parse(email.To));
            emailMessage.Subject = email.Subject;
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = email.Content + "\n" + email.Link };
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2> <a href='{1}'>Click here</a>", email.Content,email.Link) };
            return emailMessage;
        }
        //Send message async
        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(_emailConfiguration.UserName, _emailConfiguration.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    //log an error message or throw an exception or both.
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }

            }
        }
    }
}
