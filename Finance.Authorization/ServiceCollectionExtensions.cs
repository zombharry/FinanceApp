using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Finance.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFinanceAuthorization(this IServiceCollection services, IConfiguration configuration, string connectionStringName = "DefaultConnectionString")
        {
            var connectionString = configuration.GetConnectionString(connectionStringName);
            services.AddDbContext<Data.ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<Data.ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Export Razor Pages (Identity UI) from this library
            services.AddRazorPages();

            return services;
        }
    }
}
