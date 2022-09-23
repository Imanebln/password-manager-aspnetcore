using AuthenticationService;
using AuthenticationService.Models;
using Data.Models;
using EmailingService.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PasswordEncryption.Contracts;
using System.Security.Claims;
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
        private readonly IPrettyEmail _emailSender;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<AccountsController> _logger;
        private readonly ISymmetricEncryptDecrypt _encryptionService;

        public AccountsController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ITokensManager tokensManager, ILogger<AccountsController> logger, ISymmetricEncryptDecrypt encryptionService, IPrettyEmail emailSender, IHttpContextAccessor httpContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokensManager = tokensManager;
            _logger = logger;
            _encryptionService = encryptionService;
            _emailSender = emailSender;
            _httpContext = httpContext;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                // Generating a random userKey and encrypting it.
                var userKey = _encryptionService.GenerateRandomUserKey();
                (string derivedKey, string IVBase64) = _encryptionService.DeriveKeyFromPassword(userDto.Password);

                var encryptedKey = _encryptionService.EncryptUserKey(userKey, derivedKey, IVBase64);

                ApplicationUser appUser = new()
                {
                    UserName = userDto.Username,
                    Email = userDto.Email,
                    EncryptedKeyIV = IVBase64,
                    EncryptedKey = encryptedKey
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

                    // Send confirmation link
                    await ValidationEmail(appUser);


                    return Ok("User Created Successfully, please confirm email");
                }
                else
                {
                    foreach (var error in creatingUser.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpGet("send-email-confirmation")]
        public async Task<IActionResult> SendEmailConfirmation(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound("Username not found");

            await ValidationEmail(user);

            return Ok("Confirmation email sent.");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(loginDto.Username);
                if (user is null)
                    return NotFound("Username not found");

                _logger.LogInformation("Attempting to verify user's credentials");

                var isUserValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isUserValid)
                    return Unauthorized("Wrong username or password");

                if (!await _userManager.IsEmailConfirmedAsync(user))
                    return BadRequest("Please confirm your email");
                if (await _userManager.IsLockedOutAsync(user))
                    return BadRequest("Please reset your password or try again later");
                if (await _userManager.GetTwoFactorEnabledAsync(user))
                    return await GenerateOTPForTwoFactorAuth(user.Email);

                // Decrypting the user key.

                (string derivedKey, _) = _encryptionService.DeriveKeyFromPassword(loginDto.Password);

                var decryptedKey = _encryptionService.DecryptUserKey(user.EncryptedKey, derivedKey, user.EncryptedKeyIV);

                // Generate refresh token
                var refreshToken = _tokensManager.GenerateRefreshToken();

                var cookieOption = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = refreshToken.ExpirationDate,
                    Secure = true,
                    SameSite = SameSiteMode.None
                };

                _httpContext.HttpContext!.Response.Cookies.Append("decryptionKey", decryptedKey, cookieOption);


                try
                {
                    await _tokensManager.SetRefreshToken(user, refreshToken);
                }
                catch (Exception ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
                }


                _logger.LogInformation("Attempting to generate access token");
                var accessToken = await _tokensManager.GenerateToken(user);


                return Ok(new
                {
                    accessToken,
                    refreshToken,
                    decryptedKey
                });

            }

            return BadRequest(ModelState);
        }


        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenModel>> RefreshToken(TokenApiModel apiModel)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _tokensManager.GetPrincipalFromExpiredToken(apiModel.AccessToken);
            }
            catch (SecurityTokenSignatureKeyNotFoundException)
            {
                return Unauthorized("Access token is corrupted.");
            }

            var userName = principal.Identity!.Name;

            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return NotFound("User not found");

            var refreshToken = Request.Cookies["refreshToken"];

            refreshToken ??= apiModel.RefreshToken;

            if (user.RefreshToken is null || !user.RefreshToken.Token.Equals(refreshToken))
                return Unauthorized("Invalid refresh token");
            if (user.RefreshToken.ExpirationDate < DateTime.Now)
                return Unauthorized("Refresh Token Expired");

            var token = await _tokensManager.GenerateToken(user);
            var newRefreshToken = _tokensManager.GenerateRefreshToken();
            await _tokensManager.SetRefreshToken(user, newRefreshToken);

            return Ok(new
            {
                accessToken = token.AccessToken,
                refreshToken = newRefreshToken.Token
            });
        }

        [HttpGet("revoke-refresh-token"), Authorize]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            var userName = _httpContext.HttpContext!.User.Identity!.Name;

            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return NotFound("User not found");

            try
            {
                await _tokensManager.RevokeRefreshToken(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok("Successfuly revoked");
        }


        // Send email to confirm 
        private async Task<string> ValidationEmail(ApplicationUser user)
        {
            //Generate confirmation token
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // send token via email
            token = HttpUtility.UrlEncode(token);
            var confirmationLink = "https://localhost:7077/api/Accounts/confirm-email?token=" + token + "&email=" + user.Email;
            await _emailSender.SendEmailVerification(user.Email, confirmationLink);

            return "Success";
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            //check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound("User not found");

            _logger.LogInformation("Attempting to verify email");
            //confirm email via received token and email
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Redirect("http://localhost:4200/login");

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

            // added link to redirect to reset password page in front
            var confirmationlink = "http://localhost:4200/reset-password?email=" + user.Email;
            await _emailSender.SendPasswordReset(user.Email, confirmationlink, resetToken);

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

            if (user is null)
                return NotFound("User not found");

            var token = await _userManager.GenerateChangeEmailTokenAsync(user, emailChangeModel.NewEmail);

            token = HttpUtility.UrlEncode(token);
            var confirmationLink = String.Format("https://localhost:7077/api/Accounts/confirm-email-change?token={0}&oldemail={1}&newemail={2}", token, user.Email, emailChangeModel.NewEmail);

            await _emailSender.SendEmailChange(user.Email, confirmationLink);

            return Ok();
        }

        [HttpGet("confirm-email-change")]
        public async Task<IActionResult> ConfirmEmailChange(string token, string oldEmail, string newEmail)
        {
            var user = await _userManager.FindByEmailAsync(oldEmail);
            if (user is null)
                return NotFound("User not found");

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (result.Succeeded)
                return Ok("Email successfully changed");

            return BadRequest(result.Errors);
        }



        [HttpGet("Set-2fa"), Authorize]
        public async Task<IActionResult> Set2FA(bool enabled)
        {
            var userName = _httpContext.HttpContext!.User.Identity!.Name;

            var user = await _userManager.FindByNameAsync(userName);
            if (user is null)
                return NotFound("User not found");

            var result = await _userManager.SetTwoFactorEnabledAsync(user, enabled);
            if (result.Succeeded)
                return Ok("Successfully enabled 2FA");

            return BadRequest("Couldn't enable 2FA");
        }

        [HttpGet("send-2fa-code")]
        public async Task<IActionResult> GenerateOTPForTwoFactorAuth(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return NotFound("User not found");

            var providers = await _userManager.GetValidTwoFactorProvidersAsync(user);
            if (!providers.Contains("Email"))
                return Unauthorized("Invalid 2-Step Verification Provider.");

            var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
            await _emailSender.Send2FAToken(user.Email, token);

            return Ok(new { Is2FARequired = true, Provider = "Email", Username = user.UserName });
        }

        [HttpPost("2fa-login")]
        public async Task<IActionResult> LoginWithOTP(OTPLoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user is null)
                return NotFound("User not found");

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(user, loginModel.Provider, loginModel.Token);

            if (!isValid)
                return Unauthorized("Invalid token.");

            var refreshToken = _tokensManager.GenerateRefreshToken();
            try
            {
                await _tokensManager.SetRefreshToken(user, refreshToken);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            return Ok(new
            {
                accessToken = await _tokensManager.GenerateToken(user),
                refreshToken
            });
        }



    }
}
