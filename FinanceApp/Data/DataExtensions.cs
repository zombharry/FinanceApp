using Microsoft.EntityFrameworkCore;

namespace FinanceApp.Data;

public static class DataExtensions
{
    public static void AddFinanceDb(this WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnectionString");
        builder.Services.AddSqlite<FinanceContext>(connectionString);
    }
}
