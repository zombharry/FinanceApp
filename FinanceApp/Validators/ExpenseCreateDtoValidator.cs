using FinanceApp.Dtos;
using FinanceApp.Models;
using FluentValidation;

namespace FinanceApp.Validators;

public class ExpenseCreateDtoValidator : AbstractValidator<ExpenseCreateDto>
{
    public ExpenseCreateDtoValidator()
    {
        RuleFor(expense => expense.Description).NotNull();
        RuleFor(expense => expense.Amount).NotNull().GreaterThan(0);
        RuleFor(expense => expense.Category).NotNull();
        RuleFor(expense => expense.Date).NotNull();
    }
}
