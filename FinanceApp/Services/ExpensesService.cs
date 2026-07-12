using FinanceApp.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Services;

public class ExpensesService : IExpensesService
{
    private readonly FinanceContext _context;

    public ExpensesService(FinanceContext context)
    {
        this._context = context;
    }
    public async Task AddAsync(Expense expense)
    {
        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _context.Expenses.Where(expense => expense.Id == id).ExecuteDeleteAsync();
    }

    public async Task EditAsync(Expense expense)
    {
        var existingExpense = await _context.Expenses.FindAsync(expense.Id);
        if (existingExpense is null)
        {
            return;
        }

        existingExpense.Description = expense.Description;
        existingExpense.Amount = expense.Amount;
        existingExpense.Category = expense.Category;

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Expense>> GetAllAsync(string userId)
    {
        var expenses = await _context.Expenses.Where(expense => expense.UserId.Equals(userId)).ToListAsync();
        return expenses;
    }
    public async Task<Expense> GetByIdAsync(int id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        
        return expense;
    }
}
