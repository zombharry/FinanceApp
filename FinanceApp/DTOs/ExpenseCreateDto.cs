namespace FinanceApp.Dtos;

public record ExpenseCreateDto
(
    string Description,

    double Amount,

    string Category,

    DateOnly Date
);
