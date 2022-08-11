using AuthenticationService;
using AuthenticationService.Models;
using Data.Models;
using EmailingService.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public AccountsController(UserManager<ApplicationUser> userManager,RoleManager<ApplicationRole> roleManager,ITokensManager tokensManager, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokensManager = tokensManager;
            _emailService = emailService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserDto userDto)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = new ApplicationUser
                {
                    UserName = userDto.Username,
                    Email = userDto.Email
                };

                var creatingUser = await _userManager.CreateAsync(appUser, userDto.Password);
                if (creatingUser.Succeeded)
                {
                    
                    if (!await _roleManager.RoleExistsAsync(UserRoles.REGULAR))
                        await _roleManager.CreateAsync(new ApplicationRole() { Name = UserRoles.REGULAR });

                    if (await _roleManager.RoleExistsAsync(UserRoles.REGULAR))
                    {
                        await _userManager.AddToRoleAsync(appUser, UserRoles.REGULAR);
                    }
                    
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

                var isUserValid = await _userManager.CheckPasswordAsync(user,loginDto.Password);

                if(!await _userManager.IsEmailConfirmedAsync(user))
                    return BadRequest("Please confirm your email");
                if (await _userManager.IsLockedOutAsync(user))
                    return BadRequest("Please reset your password or try again later");

                
                

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

           var result =  await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
                return Ok("Email confirmed");

            return BadRequest("Could not confirm email");

        }
    }
}
