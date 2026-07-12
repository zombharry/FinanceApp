using FinanceApp.Data;
using FluentValidation;

namespace FinanceApp.Validators;

public class ExpenseGetValidator : AbstractValidator<Expense>
{
    public ExpenseGetValidator()
    {
        RuleFor(expense => expense.Id).NotNull().GreaterThan(0);
        RuleFor(expense => expense.Description).NotNull();
        RuleFor(expense => expense.Amount).NotNull().GreaterThan(0);
        RuleFor(expense => expense.Category).NotNull();
        RuleFor(expense => expense.Date).NotNull();
    }
}
