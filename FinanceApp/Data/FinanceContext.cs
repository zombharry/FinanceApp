using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public class FinanceContext(DbContextOptions<FinanceContext> options) : DbContext (options)
{
    public DbSet<Expense> Expenses => Set<Expense>();
}
