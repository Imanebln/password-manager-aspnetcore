using AuthenticationService.Models;
using Data.Models;
using System.Security.Claims;

namespace AuthenticationService
{
    public interface ITokensManager
    {
         Task<AccessTokenModel> GenerateToken(ApplicationUser user);
         RefreshTokenModel GenerateRefreshToken();
         Task SetRefreshToken(ApplicationUser user,RefreshTokenModel refreshToken);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        Task RevokeRefreshToken(ApplicationUser user);
    }
}