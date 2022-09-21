using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PasswordManager.ActionFilters
{
    public class GetCurrentUserActionFilter : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetCurrentUserActionFilter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var userName = context.HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
            {
                context.Result = new NotFoundResult();
            }
            else
            {
                context.HttpContext.Items.Add(nameof(user), user);
            }

            await next();
        }
    }
}
