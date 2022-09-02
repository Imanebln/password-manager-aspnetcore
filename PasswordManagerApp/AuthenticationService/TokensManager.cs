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
                ExpirationDate = DateTime.Now.AddDays(7),
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
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return new AccessTokenModel(jwt, DateTime.Now, DateTime.UtcNow.AddDays(1), userRoles);

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
    }
}
