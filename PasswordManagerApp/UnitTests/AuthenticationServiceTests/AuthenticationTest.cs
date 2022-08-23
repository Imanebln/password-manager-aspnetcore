using AuthenticationService;
using AuthenticationService.Models;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests.AuthenticationServiceTests
{
    public class AuthenticationTest
    {
        private readonly TokensManager _tokensManager;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUser _user;
        protected readonly ITestOutputHelper _output;

        public AuthenticationTest(ITestOutputHelper output)
        {
            _tokensManager = new TokensManager(_configuration,_httpContext,_userManager);
            _output = output;
            _user = new ApplicationUser()
            {
                Email = "imane.boulouane@ump.ac.ma",
                UserName = "imaneboulouane",

            };
        }

        [Fact]
        public void GenerateTokenTest()
        {
            // Act
            var result = _tokensManager.GenerateToken(_user);
            // Assert
            Assert.NotNull(result);
            Assert.IsType<Task<AccessTokenModel>>(result);
        }
    }
}
