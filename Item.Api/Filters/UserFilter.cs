using Item.Api.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Item.Api.Filters
{
    public class UserFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var callerId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (context.ActionArguments.TryGetValue("productEditDto", out var dtoObj)
                && dtoObj is ProductEditDTO dto
                && !string.Equals(dto.OwnerId, callerId, StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ForbidResult();
                return;
            }

            await next();
        }
    }
}
