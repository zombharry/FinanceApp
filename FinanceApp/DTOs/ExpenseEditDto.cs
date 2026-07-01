namespace FinanceApp.Dtos;

public record ExpenseEditDto
(
    int Id,

    string Description,

    double Amount,

    string Category,

    DateOnly Date
);
