using FinanceApp.Data;
using FinanceApp.Dtos;
using FinanceApp.Filters;
using FinanceApp.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinanceApp.Controllers;

[Authorize]
public class ExpensesController : Controller
{
    private readonly IExpensesService _expensesService;
    private readonly IValidator<ExpenseEditDto> _expenseEditValidator;
    private readonly IValidator<ExpenseCreateDto> _expenseCreateValidator;
    private readonly IValidator<Expense> _expenseGetValidator;

    public ExpensesController(IExpensesService expensesService,
        IValidator<ExpenseEditDto> expenseEditValidator,
        IValidator<ExpenseCreateDto> expenseCreateValidator,
        IValidator<Expense> expenseGetValidator
        )
    {
        _expensesService = expensesService;
        _expenseEditValidator = expenseEditValidator;
        _expenseCreateValidator = expenseCreateValidator;
        _expenseGetValidator = expenseGetValidator;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var expenses = await _expensesService.GetAllAsync(userId);

        var expenseDtos = expenses.Select(expense =>
        new ExpenseGetDto(
           expense.Id,
           expense.Description,
           expense.Amount,
           expense.Category,
           DateOnly.FromDateTime(expense.Date)
       ));

        return View(expenseDtos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [ServiceFilter(typeof(UserFilter))]
    public async Task<IActionResult> Edit(int id, string returnUrl)
    {
        var expense = await _expensesService.GetByIdAsync(id);
        var validatorResult = await _expenseGetValidator.ValidateAsync(expense);
        if (!validatorResult.IsValid)
        {
            TempData["error"] = string.Join(
                "<br/>",
                validatorResult.Errors.Select(x => x.ErrorMessage));
            return Redirect(returnUrl ?? "/");
        }

        var expenseEditDto = new ExpenseEditDto(
            id,
            expense.Description,
            expense.Amount,
            expense.Category,
            DateOnly.FromDateTime(expense.Date)
            );
        
        return View(expenseEditDto);
    }

    public IActionResult Error()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ExpenseCreateDto expenseDto)
    {
        var validatorResult = await _expenseCreateValidator.ValidateAsync(expenseDto);
        if (!(validatorResult.IsValid))
        {
            TempData["error"] = string.Join(
                "<br/>",
                validatorResult.Errors.Select(x => x.ErrorMessage));

            return View(expenseDto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var expense = new Expense
        { 
            Description = expenseDto.Description,
            Amount = expenseDto.Amount,
            Category = expenseDto.Category,
            Date = DateTime.UtcNow,
            UserId = userId
        };

        TempData["success"] = "Expense has been created successfully";

        await _expensesService.AddAsync(expense);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ServiceFilter(typeof(UserFilter))]
    public async Task<ActionResult> Edit(ExpenseEditDto expenseEditDto, string returnUrl)
    {
        var validatorResult = await _expenseEditValidator.ValidateAsync(expenseEditDto);
        if (!(validatorResult.IsValid))
        {
            TempData["error"] = string.Join(
                "<br/>",
                validatorResult.Errors.Select(x => x.ErrorMessage));

            return Redirect(returnUrl ?? "/");
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var expense = new Expense
        {
            Id = expenseEditDto.Id,
            Description = expenseEditDto.Description,
            Amount = expenseEditDto.Amount,
            Category = expenseEditDto.Category,
            Date = expenseEditDto.Date.ToDateTime(new TimeOnly(0, 0, 0)),
            UserId = userId
        };

        await _expensesService.EditAsync(expense);
        TempData["success"] = "Expense has been updated successfully";
        return RedirectToAction("Index");
    }

    [ServiceFilter(typeof(UserFilter))]
    public async Task<ActionResult> Delete(int id, string returnUrl)
    {
        await _expensesService.DeleteAsync(id);
        TempData["success"] = "Expense has been deleted successfully";
        return RedirectToAction("Index");
    }
}
