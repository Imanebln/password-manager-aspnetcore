using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PasswordManager.ActionFilters
{
    public class GetUserEncryptionKeyActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var encryptionKey = context.HttpContext.Request.Cookies["decryptionKey"];

            if (encryptionKey is null)
                context.Result = new BadRequestObjectResult("An error occured.");
            else context.HttpContext.Items.Add(nameof(encryptionKey), encryptionKey);


            await next();
        }
    }
}
