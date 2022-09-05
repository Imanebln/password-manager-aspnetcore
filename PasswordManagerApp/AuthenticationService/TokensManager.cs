using AuthenticationService.Models;
using Data.DataAccess;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class TokensManager : ITokensManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserDataRepository _userData;

        public TokensManager(IConfiguration configuration, IHttpContextAccessor httpContext,UserManager<ApplicationUser> userManager, IUserDataRepository userData)
        {
            _configuration = configuration;
            _httpContext = httpContext;
            _userManager = userManager;
            _userData = userData;
        }

        /// <summary>
        /// Generate a refresh token for a user. 
        /// </summary>
        /// <returns>Refresh Token model.</returns>
        public RefreshTokenModel GenerateRefreshToken()
        {
            var refreshToken = new RefreshTokenModel
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpirationDate = DateTime.Now.AddDays(30),
                CreationDate = DateTime.Now
            };

            return refreshToken;
        }


        /// <summary>
        /// Generate JWT token for a given user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>string representing the generated token</returns>
        public async Task<AccessTokenModel> GenerateToken(ApplicationUser user)
        {

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            

            //generating the secret key using appsettings's key
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value));

            //creating credentials using the previous key
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return new AccessTokenModel(jwt, DateTime.Now, DateTime.UtcNow.AddMinutes(30), userRoles);

        }

        /// <summary>
        /// Sets a refresh token to a user
        /// </summary>
        /// <param name="refreshToken">Refresh Token to add.</param>
        /// <param name="user">User whom we want to set his refresh token.</param>
        public async Task SetRefreshToken(ApplicationUser user,RefreshTokenModel refreshToken)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpirationDate
            };

            _httpContext.HttpContext.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOption);
           

                var userModel = await _userData.GetUserById(user.Id);

                if (userModel is null)
                    throw new NullReferenceException("Could not find user.");

                if (userModel.RefreshToken is null)
                    userModel.RefreshToken = new RefreshTokenModel();

                userModel.RefreshToken.Token = refreshToken.Token;
                userModel.RefreshToken.CreationDate = refreshToken.CreationDate;
                userModel.RefreshToken.ExpirationDate = refreshToken.ExpirationDate;

                await _userData.UpdateUser(userModel, user.Id);

        }

        /// <summary>
        /// This methods gets the principal from an expired access token.
        /// </summary>
        /// <param name="token">The expired access token.</param>
        /// <returns>ClaimsPrincipal of the access token.</returns>
        /// <exception cref="SecurityTokenException">Throws SecurityTokenException when access token is in invalid format.</exception>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value)),
                ValidateLifetime = false, //here we are saying that we don't care about the token's expiration date
                
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }

        public async Task RevokeRefreshToken(ApplicationUser user)
        {
            var userModel = await _userData.GetUserById(user.Id);

            if (userModel is null)
                throw new NullReferenceException("Could not find user.");

            userModel.RefreshToken = null;

            await _userData.UpdateUser(userModel, userModel.Id);
        }
    }
}
