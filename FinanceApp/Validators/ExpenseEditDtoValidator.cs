using FinanceApp.Dtos;
using FinanceApp.Models;
using FluentValidation;

namespace FinanceApp.Validators;

public class ExpenseEditDtoValidator : AbstractValidator<ExpenseEditDto>
{
    public ExpenseEditDtoValidator()
    {
        RuleFor(expense => expense.Id).NotNull().GreaterThan(0);
        RuleFor(expense => expense.Description).NotNull();
        RuleFor(expense => expense.Amount).NotNull().GreaterThan(0);
        RuleFor(expense => expense.Category).NotNull();
        RuleFor(expense => expense.Date).NotNull();
    }
}
