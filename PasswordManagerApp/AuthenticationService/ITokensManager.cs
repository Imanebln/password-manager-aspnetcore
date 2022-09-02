using AuthenticationService.Models;
using Data.Models;

namespace AuthenticationService
{
    public interface ITokensManager
    {
         Task<AccessTokenModel> GenerateToken(ApplicationUser user);
         RefreshTokenModel GenerateRefreshToken();
         Task SetRefreshToken(ApplicationUser user,RefreshTokenModel refreshToken);
    }
}