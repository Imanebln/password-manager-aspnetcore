using Data.Models;
using Data.Models.Email;
using EmailingService.Contracts;
using EmailingService.Impl;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Priority;

namespace UnitTests.EmailingTests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class EmailingTest
    {
        private readonly EmailService _emailService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly Email _emailAddress;
        private readonly ApplicationUser _user;

        public EmailingTest()
        {
            _emailConfiguration = new EmailConfiguration();
            _emailService = new EmailService(_emailConfiguration,_userManager);
            _emailAddress = new Email()
            {
                From = "",
                To = "boulouane.imane@gmail.com",
                Content = "Please Cofirm your email!",
                Link = "",
                Subject = "Email validation"

            };
            _user = new ApplicationUser()
            {
                Email = "boulouane.imane@gmail.com",

            };
        }

        [Fact, Priority(1)]
        public void SendEmailAsyncTest()
        {
            // Act
            var result = _emailService.SendEmailAsync(_emailAddress);

            // Assert
            Assert.NotNull(result);
        }
        [Fact, Priority(2)]
        public void EmailValidation()
        {
            // Act
            var result = _emailService.EmailValidation(_user);

            // Assert
            Assert.IsType<Task<string>>(result);
        }

    }
}
