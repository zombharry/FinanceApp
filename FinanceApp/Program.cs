using FinanceApp.Data;
using FinanceApp.Data.Service;
using FinanceApp.Exceptions;
using FinanceApp.Handlers;
using FinanceApp.Models;
using FinanceApp.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FinanceApp.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnectionString");
var identityConnection = builder.Configuration.GetConnectionString("IdentityContext"); 

// Use SQL Server for both application and identity databases
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlite(identityConnection));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddDbContext<FinanceContext>(options => options.UseSqlite(defaultConnection));
builder.Services.AddScoped<IExpensesService, ExpensesService>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

builder.Services.AddScoped<UserFilter>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
    };
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Logging.AddOpenTelemetry(logging =>
{
    logging.IncludeFormattedMessage = true;
    logging.IncludeScopes = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseExceptionHandler();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();
app.UseExceptionHandler();

app.UseStatusCodePages();

app.Run();
