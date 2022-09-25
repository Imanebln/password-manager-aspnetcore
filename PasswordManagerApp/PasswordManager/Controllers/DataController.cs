using Data.DataAccess;
using Data.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PasswordEncryption.Contracts;
using PasswordManager.ActionFilters;
using PasswordManager.DTO.UserData;
using PDFService.contracts;

namespace PasswordManager.Controllers
{
    [ServiceFilter(typeof(GetCurrentUserActionFilter))]
    [ServiceFilter(typeof(GetUserEncryptionKeyActionFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly IUserDataRepository _userData;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISymmetricEncryptDecrypt _symmetricEncryptDecrypt;
        private readonly IDataSummary _dataSummary;
        private readonly IUserVaultRepository _userVault;

        public DataController(IUserDataRepository userData,
                              UserManager<ApplicationUser> userManager,
                              ISymmetricEncryptDecrypt symmetricEncryptDecrypt,
                              IDataSummary dataSummary,
                              IUserVaultRepository userVault)
        {
            _userData = userData;
            _userManager = userManager;
            _symmetricEncryptDecrypt = symmetricEncryptDecrypt;
            _dataSummary = dataSummary;
            _userVault = userVault;
        }




        [HttpDelete("delete-current-user-data"), Authorize]
        public async Task<IActionResult> DeleteCurrentUserData()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            await _userData.DeleteDataByUserId(user.Id);

            return NoContent();
        }

        [HttpGet("generate-pdf-summary"),Authorize]
        public async Task<IActionResult> GenerateUserDataSummaryPDF()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;
            var encryptionKey = HttpContext.Items["encryptionKey"] as string;

            var fileBytes = await _dataSummary.GeneratePDFSummary(user, encryptionKey);

            return File(fileBytes,"application/pdf","summary.pdf");
        }

        [HttpGet("generate-images-summary"),Authorize]
        public async Task<IActionResult> GenerateUserDataSummaryImages()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;
            var encryptionKey = HttpContext.Items["encryptionKey"] as string;

            var fileBytes = await _dataSummary.GenerateImageSummary(user, encryptionKey);

            return File(fileBytes.First(), "image/png", "summary.png");
        }

        [HttpGet("generate-text-summary"),Authorize]
        public async Task<IActionResult> GenerateUserDataSummaryText()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;
            var encryptionKey = HttpContext.Items["encryptionKey"] as string;

            var fileBytes = await _dataSummary.GenerateTextFileSummary(user, encryptionKey);

            return File(fileBytes, "text/plain", "summary.txt");
        }

        [HttpGet("passwords/{id}"),Authorize]
        public async Task<IActionResult> GetPasswordById(Guid id)
        {
            var user = HttpContext.Items["user"] as ApplicationUser;
            var encryptionKey = Request.Cookies["decryptionKey"];

            if (encryptionKey is null)
                return BadRequest("An error occured");



            var result = await _userVault.GetPasswordById(id,user.Id);
            if (result is null)
                return NotFound("Password not found");

            result.EncryptedPassword = _symmetricEncryptDecrypt.Decrypt(result.EncryptedPassword, result.EncryptedPasswordIV, encryptionKey);
            return Ok(result);
        }

        [HttpGet("passwords"), Authorize]
        public async Task<IActionResult> GetAllPasswords()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;
            var encryptionKey = HttpContext.Items["encryptionKey"] as string;

            var result = await _userVault.GetUserVault(user.Id);
            result = result.Select(p => { p.EncryptedPassword = _symmetricEncryptDecrypt.Decrypt(p.EncryptedPassword, p.EncryptedPasswordIV, encryptionKey); return p; });
            return Ok(result);
        }

        [HttpPut("passwords"), Authorize]
        public async Task<IActionResult> UpdatePasswordById(Guid id,AccountInfosModel accountsInfos)
        {
            if (!id.Equals(accountsInfos.Id))
                return BadRequest("Cannot change id of an entity.");

            var user = HttpContext.Items["user"] as ApplicationUser;
            var encryptionKey = Request.Cookies["decryptionKey"];
            string IVKey;

            if (encryptionKey is null)
                return BadRequest("An error occured");

            var oldPassword = await _userVault.GetPasswordById(id, user.Id);

            if (oldPassword is null)
                return NotFound(String.Format("The password with id {0} is not found.", id));

            accountsInfos.EncryptedPassword = _symmetricEncryptDecrypt.Encrypt(accountsInfos.EncryptedPassword,accountsInfos.EncryptedPasswordIV,encryptionKey);


            await _userVault.UpdatePasswordById(id, user.Id,accountsInfos);
            return StatusCode(StatusCodes.Status202Accepted);
        }

        [HttpPost("passwords"), Authorize]
        public async Task<IActionResult> InsertPassword(AccountsInfosModelInsertDTO accountsInfosDto)
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            var encryptionKey = Request.Cookies["decryptionKey"];
            string IVKey;

            if (encryptionKey is null)
                return BadRequest("An error occured");

            var accountsInfosModel = accountsInfosDto.Adapt<AccountInfosModel>();

            IVKey = _symmetricEncryptDecrypt.GenerateIVFromKey(encryptionKey);
            accountsInfosModel.EncryptedPasswordIV = IVKey;

            // Encrypt password
            accountsInfosModel.EncryptedPassword = _symmetricEncryptDecrypt.Encrypt(accountsInfosModel.EncryptedPassword, IVKey, encryptionKey);


            await _userVault.InsertPassword(accountsInfosModel, user.Id);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("passwords/{id}"),Authorize]
        public async Task<IActionResult> DeletePasswordById(Guid id)
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            await _userVault.DeletePassword(id, user.Id);

            return NoContent();
        }
    }
}
