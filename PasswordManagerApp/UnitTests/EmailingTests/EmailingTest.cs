using Data.Models.Email;
using EmailingService.Impl;
using Xunit;
using Xunit.Priority;

namespace UnitTests.EmailingTests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class EmailingTest
    {
        private readonly EmailService _emailService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly Email _emailAddress;

        public EmailingTest()
        {
            _emailConfiguration = new EmailConfiguration();
            _emailService = new EmailService(_emailConfiguration);
            _emailAddress = new Email()
            {
                From = "",
                To = "boulouane.imane@gmail.com",
                Content = "Please Cofirm your email!",
                Link = "",
                Subject = "Email validation"

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

    }
}
