using FinanceApp.Data;
using FinanceApp.Data.Service;
using FinanceApp.Dtos;
using FinanceApp.Exceptions;
using FinanceApp.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Controllers;

public class ExpensesController : Controller
{
    private readonly IExpensesService _expensesService;
    private readonly IValidator<ExpenseEditDto> _expenseEditValidator;
    private readonly IValidator<ExpenseCreateDto> _expenseCreateValidator;
    private readonly IValidator<Expense> _expenseGetValidator;
    private readonly IValidator<int> _existingIdValidator;

    public ExpensesController(IExpensesService expensesService,
        IValidator<ExpenseEditDto> expenseEditValidator,
        IValidator<ExpenseCreateDto> expenseCreateValidator,
        IValidator<Expense> expenseGetValidator,
        IValidator<int> existingIdValidator
        )
    {
        _expensesService = expensesService;
        _expenseEditValidator = expenseEditValidator;
        _expenseCreateValidator = expenseCreateValidator;
        _expenseGetValidator = expenseGetValidator;
        _existingIdValidator = existingIdValidator;
    }

    public async Task<IActionResult> Index()
    {
        var expenses = await _expensesService.GetAllAsync();

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

    public async Task<IActionResult> Edit(int id, string returnUrl)
    {
        var expenseExists = await _existingIdValidator.ValidateAsync(id);
        if (!expenseExists.IsValid)
        {
            TempData["error"] = string.Join(
                "<br/>",
                expenseExists.Errors.Select(x => x.ErrorMessage));
            return Redirect(returnUrl ?? "/");
        }
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
        var expense = new Expense
        { 
            Description = expenseDto.Description,
            Amount = expenseDto.Amount,
            Category = expenseDto.Category,
            Date = DateTime.UtcNow
        };

        TempData["success"] = "Expense has been created successfully";

        await _expensesService.AddAsync(expense);
        return RedirectToAction("Index");
    }

    [HttpPost]
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
        var expense = new Expense
        {
            Id = expenseEditDto.Id,
            Description = expenseEditDto.Description,
            Amount = expenseEditDto.Amount,
            Category = expenseEditDto.Category,
            Date = expenseEditDto.Date.ToDateTime(new TimeOnly(0, 0, 0))
        };

        await _expensesService.EditAsync(expense);
        TempData["success"] = "Expense has been updated successfully";
        return RedirectToAction("Index");
    }

    public async Task<ActionResult> Delete(int id, string returnUrl)
    {
        var expenseExists = await _existingIdValidator.ValidateAsync(id);
        if (!expenseExists.IsValid)
        {
            TempData["error"] = string.Join(
                "<br/>",
                expenseExists.Errors.Select(x => x.ErrorMessage));
            return Redirect(returnUrl ?? "/");
        }

        await _expensesService.DeleteAsync(id);
        TempData["success"] = "Expense has been deleted successfully";
        return RedirectToAction("Index");
    }
}
