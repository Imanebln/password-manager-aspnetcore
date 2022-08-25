using Microsoft.Extensions.DependencyInjection;
using System;
using PasswordEncryption.Impl;
using PasswordEncryption.Contracts;
using EmailingService.Impl;
using Data.Models.Email;
using Data.Models;
using AuthenticationService;
using PasswordManager.Controllers;
using Microsoft.AspNetCore.Identity;
using EmailingService.Contracts;
using Microsoft.Extensions.Logging;

namespace UnitTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            /* services.AddSingleton<ISymmetricEncryptDecrypt,SymmetricEncryptDecrypt>();
             services.AddSingleton<ITokensManager, TokensManager>();*/

            

                
        }
    }
}
