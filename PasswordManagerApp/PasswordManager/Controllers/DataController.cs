using Data.DataAccess;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public DataController(IUserDataRepository userData, IHttpContextAccessor httpContext, UserManager<ApplicationUser> userManager )
        {
            _userData = userData;
            _httpContext = httpContext;
            _userManager = userManager;
        }


        [HttpGet("get-current-user-data"),Authorize]
        public async Task<ActionResult<UserDataModel>> GetCurrentUserData()
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            var userData = await _userData.GetDataByUserId(user.Id);
            if(userData is null)
                return NotFound("Could not find any data");

            return Ok(userData);
        }

        [HttpPost("insert-current-user-data"),Authorize]
        public async Task<ActionResult> InsertCurrentUserData(UserDataModel userDataModel)
        {
            var user = HttpContext.Items["user"] as ApplicationUser;

            var oldeData = await _userData.GetDataByUserId(user.Id);
            if (oldeData is not null)
                return BadRequest("A user cannot have multiple data.");

            userDataModel.UserId = user.Id;
            userDataModel.AccountInfos = userDataModel.AccountInfos.Select(a => { a.Id = Guid.NewGuid(); return a; });


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
