using AuthenticationService;
using AuthenticationService.Models;
using Data.DataAccess;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
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
        private readonly RefreshTokenModel _refreshTokenModel;
        protected readonly ITestOutputHelper _output;
        private readonly IUserDataRepository userData;

        public AuthenticationTest(ITestOutputHelper output)
        {
            _tokensManager = new TokensManager(_configuration, _httpContext, _userManager, userData);
            _output = output;
            _user = new ApplicationUser()
            {
                Email = "imane.boulouane@ump.ac.ma",
                UserName = "imaneboulouane",

            };

            _refreshTokenModel = new RefreshTokenModel()
            {
                Token = "",
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
        [Fact]
        public void GenerateRefreshTokenTest()
        {
            // Act
            var result = _tokensManager.GenerateRefreshToken();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RefreshTokenModel>(result);

        }

        [Fact]
        public void SetRefreshTokenTest()
        {
            // Act
            var result = _tokensManager.SetRefreshToken(_user, _refreshTokenModel);

            // Assert
            Assert.Equal(_refreshTokenModel.Token, _user.RefreshToken?.Token);
        }

        [Fact]
        public void GetPrincipalFromExpiredTokenTest()
        {
            // Act
            string token = "";
            var result = _tokensManager.GetPrincipalFromExpiredToken(token);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ClaimsPrincipal>(result);
        }

        [Fact]
        public void RevokeRefreshTokenTest()
        {
            // Act
            var result = _tokensManager.RevokeRefreshToken(_user);

            // Assert
            Assert.Null(_user.RefreshToken);
        }
    }
}
