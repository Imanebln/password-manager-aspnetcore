using AuthenticationService.Models;
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
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService
{
    public class TokensManager : ITokensManager
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokensManager(IConfiguration configuration, IHttpContextAccessor httpContext,UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _httpContext = httpContext;
            _userManager = userManager;
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
    }
}
