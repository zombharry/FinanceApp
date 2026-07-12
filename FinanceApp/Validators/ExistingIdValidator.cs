using FinanceApp.Dtos;
using FinanceApp.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Internal;

namespace FinanceApp.Validators;

public class ExistingIdValidator : AbstractValidator<int>
{
    private readonly IExpensesService _expensesService;
    public ExistingIdValidator(IExpensesService expensesService)
    {
        _expensesService = expensesService;

        RuleFor(x => x)
            .NotNull()
            .GreaterThan(0)
            .MustAsync(ExpenseExistsAsync).WithMessage(x => $"Expense {x} was not found");
    }

    private async Task<bool> ExpenseExistsAsync(int id, CancellationToken cancellationToken)
    {
        var expense = await _expensesService.GetByIdAsync(id);
        return expense is not null;
    }
}
