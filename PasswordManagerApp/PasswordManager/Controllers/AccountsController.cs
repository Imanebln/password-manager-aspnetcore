﻿using AuthenticationService;
using AuthenticationService.Models;
using Data.Models;
using Data.Models.Email;
using EmailingService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace PasswordManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ITokensManager _tokensManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountsController> _logger;
        private readonly EmailConfiguration _emailConfiguration;

        public AccountsController(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager,ITokensManager tokensManager, IEmailService emailService,ILogger<AccountsController> logger, EmailConfiguration emailConfiguration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokensManager = tokensManager;
            _emailService = emailService;
            _logger = logger;
            _emailConfiguration = emailConfiguration;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new()
                {
                    UserName = userDto.Username,
                    Email = userDto.Email
                };

                _logger.LogInformation("Attemting to create a user");
                var creatingUser = await _userManager.CreateAsync(appUser, userDto.Password);
                if (creatingUser.Succeeded)
                {
                    
                    if (!await _roleManager.RoleExistsAsync(UserRoles.REGULAR))
                        await _roleManager.CreateAsync(new ApplicationRole() { Name = UserRoles.REGULAR });

                    if (await _roleManager.RoleExistsAsync(UserRoles.REGULAR))
                    {
                        await _userManager.AddToRoleAsync(appUser, UserRoles.REGULAR);
                    }

                    _logger.LogInformation("Sending confirmation Email");
                    await _emailService.EmailValidation(appUser);

                    return Ok("User Created Successfully, please confirm email");
                }                  
                else
                {
                    foreach (var error in creatingUser.Errors)
                        ModelState.AddModelError(error.Code , error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginDto.Username);
                if (user is null)
                    return NotFound("Username not found");

                _logger.LogInformation("Attempting to verify user's credentials");

                var isUserValid = await _userManager.CheckPasswordAsync(user,loginDto.Password);
                if(!isUserValid)
                    return Unauthorized("Wrong username or password");

                if(!await _userManager.IsEmailConfirmedAsync(user))
                    return BadRequest("Please confirm your email");
                if (await _userManager.IsLockedOutAsync(user))
                    return BadRequest("Please reset your password or try again later");



                _logger.LogInformation("Attempting to generate access token");
                return Ok(await _tokensManager.GenerateToken(user));

            }

            return BadRequest(ModelState);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token,string email)
        {
            //check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound("User not found");

            _logger.LogInformation("Attempting to verify email");
            //confirm email via received token and email
           var result =  await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Redirect("https://localhost:7077/swagger/index.html");

            _logger.LogError("Email not confirmed");
            return StatusCode(StatusCodes.Status500InternalServerError, "Could not confirm email");

        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestNewPassword(RequestPasswordResetModel requestPasswordResetModel)
        {
            var user = await _userManager.FindByEmailAsync(requestPasswordResetModel.Email);
            if (user is null)
                return NotFound("User not found");

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            var email = new Email
            {
                From = _emailConfiguration.From,
                To = user.Email,
                Content = $"Please copy the following token and use it to reset your password: {resetToken}"
            };
            await _emailService.SendEmailAsync(email);

            return Ok("Please check your email to reset your password.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordModel.Email);
            if (user is null)
                return NotFound("User not found");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordModel.Token, resetPasswordModel.NewPassword);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, "Could not reset password, please try again later");

            return Ok("You've successfully reseted your password");
        }

        [HttpPost("request-email-change")]
        public async Task<IActionResult> RequestEmailChange(RequestEmailChangeModel emailChangeModel)
        {
            var user = await _userManager.FindByEmailAsync(emailChangeModel.Email);

            if(user is null)
                return NotFound("User not found");

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, emailChangeModel.NewEmail);

            token = HttpUtility.UrlEncode(token);
            var confirmationLink = String.Format("https://localhost:7077/api/Accounts/confirm-email-change?token={0}&oldemail={1}&newemail={2}", token,user.Email,emailChangeModel.NewEmail);

            Email email = new()
            {
                To = user.Email,
                Subject = "Confirm email change",
                Content = string.Format("<h2 style='color: red;'>Confirm changing your email, please click this link:</h2>"),
                Link = confirmationLink,
                From = _emailConfiguration.From
            };
            await _emailService.SendEmailAsync(email);

            return Ok();
        }

        [HttpGet("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange(string token,string oldEmail,string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(oldEmail);
            if (user is null)
                return NotFound("User not found");

            var result = await _userManager.ChangeEmailAsync(user,newEmail,token);
            if(result.Succeeded)
                return Ok("Email successfully changed");

            return BadRequest(result.Errors);
        }

        //TODO: email change 
        //TODO: refresh token 
        //TODO: 2FA
        

    }
}
