namespace FinanceApp.Dtos;

public record ExpenseGetDto
(
    int Id,

    string Description,

    double Amount,

    string Category,

    DateOnly Date
);
