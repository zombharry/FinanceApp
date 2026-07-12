using FinanceApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Security.Claims;

namespace FinanceApp.Filters;

public class UserFilter : IAsyncActionFilter
{
    private readonly IExpensesService _expenseService;

    public UserFilter(IExpensesService expenseService)
    {
        _expenseService = expenseService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ActionArguments.TryGetValue("id", out var idObj)
            || idObj is not int id)
        {
            var tempData = context.HttpContext.RequestServices
                .GetRequiredService<ITempDataDictionaryFactory>()
                .GetTempData(context.HttpContext);
            tempData["error"] = "Unknown Error";

            context.Result = new BadRequestResult();
            return;
        }

        var result = await _expenseService.GetByIdAsync(id);
        if (result is null)
        {
            var tempData = context.HttpContext.RequestServices
                .GetRequiredService<ITempDataDictionaryFactory>()
                .GetTempData(context.HttpContext);
            tempData["error"] = "Not Found";

            context.Result = new NotFoundResult();
            return;
        }

        var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);

        if (!result.UserId.Equals(userId.Value))
        {
            var tempData = context.HttpContext.RequestServices
                .GetRequiredService<ITempDataDictionaryFactory>()
                .GetTempData(context.HttpContext);
            tempData["error"] = "Not Access";

            context.Result = new RedirectToActionResult("Index", "Expenses", null);
            return;
        }

        context.HttpContext.Items["ResolvedTask"] = result;

        await next();
    }
}
