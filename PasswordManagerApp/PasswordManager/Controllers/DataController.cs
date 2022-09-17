using Data.DataAccess;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PasswordEncryption.Contracts;
using PasswordManager.ActionFilters;

namespace PasswordManager.Controllers
{
    [ServiceFilter(typeof(GetCurrentUserActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IUserDataRepository _userData;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISymmetricEncryptDecrypt _symmetricEncryptDecrypt;

        public DataController(IUserDataRepository userData, IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager, ISymmetricEncryptDecrypt symmetricEncryptDecrypt )
        {
            _userData = userData;
            _httpContext = httpContext;
            _userManager = userManager;
            _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
        }


        [HttpGet("get-current-user-data"),Authorize]
        public async Task<ActionResult<UserDataModel>> GetCurrentUserData()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            var userData = await _userData.GetDataByUserId(user.Id);
            if(userData is null)
                return NotFound("Could not find any data");

            var decryptionKey = Request.Cookies["decryptionKey"];

            if (decryptionKey is null)
                return BadRequest("An error occured");
            // We decrypt user data.
            if (userData.AccountInfos is not null && userData.AccountInfos.Any())
                userData.AccountInfos = userData.AccountInfos.Select(ai => { ai.EncryptedPassword = _symmetricEncryptDecrypt.Decrypt(ai.EncryptedPassword, ai.EncryptedPasswordIV, decryptionKey); return ai; });

            return Ok(userData);
        }

        [HttpPost("insert-current-user-data"),Authorize]
        public async Task<ActionResult> InsertCurrentUserData(UserDataModel userDataModel)
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            var oldeData = await _userData.GetDataByUserId(user.Id);
            if (oldeData is not null)
                return BadRequest("A user cannot have multiple data.");

            var encryptionKey = Request.Cookies["decryptionKey"];
            string IVKey = string.Empty;

            if (encryptionKey is null)
                return BadRequest("An error occured");

            IVKey = _symmetricEncryptDecrypt.GenerateIVFromKey(encryptionKey);

            // Encrypt all passwords
            if (userDataModel.AccountInfos is not null)
                userDataModel.AccountInfos = userDataModel.AccountInfos.Select(ai => { ai.Id = Guid.NewGuid();  ai.EncryptedPasswordIV = IVKey; ai.EncryptedPassword = _symmetricEncryptDecrypt.Encrypt(ai.EncryptedPassword, IVKey, encryptionKey); return ai; });

            userDataModel.UserId = user.Id;


            await _userData.InsertData(userDataModel);

            return Ok(userDataModel);
        }

        [HttpPut("update-current-user-data"),Authorize]
        public async Task<IActionResult> UpdateCurrentUserData(UserDataModel userDataModel)
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            var oldUserData = await _userData.GetDataByUserId(user.Id);
            if (oldUserData is null)
                return NotFound("Could not find older data.");

            if(oldUserData.UserId != userDataModel.UserId)
                return BadRequest("You cannot change id of user");

            

            if (oldUserData.Id != userDataModel.Id)
                return BadRequest("You cannot change id of a record");


            var encryptionKey = Request.Cookies["decryptionKey"];
            string IVKey = string.Empty;

            if (encryptionKey is null)
                return BadRequest("An error occured");

            IVKey = _symmetricEncryptDecrypt.GenerateIVFromKey(encryptionKey);

            // We encrypt newly Inserted data
            if(userDataModel.AccountInfos is not null && userDataModel.AccountInfos.Any())
                userDataModel.AccountInfos = userDataModel.AccountInfos.Select(ai => { ai.Id = Guid.NewGuid(); ai.EncryptedPasswordIV = IVKey; ai.EncryptedPassword = _symmetricEncryptDecrypt.Encrypt(ai.EncryptedPassword, IVKey, encryptionKey); return ai; });

            await _userData.UpdateData(userDataModel, oldUserData.Id);

            return NoContent();
        }

        [HttpDelete("delete-current-user-data"),Authorize]
        public async Task<IActionResult> DeleteCurrentUserData()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            await _userData.DeleteDataByUserId(user.Id);

            return NoContent();
        }
    }
}
