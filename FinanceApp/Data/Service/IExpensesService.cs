using FinanceApp.Models;

namespace FinanceApp.Data.Service;

public interface IExpensesService
{
    Task<IEnumerable<Expense>> GetAllAsync(string userId);

    Task<Expense> GetByIdAsync(int id);

    Task AddAsync(Expense expense);

    Task EditAsync(Expense expense);

    Task DeleteAsync(int id);
}
