using Client.App.Components;
using Client.App.Security;
using Client.App.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var jwtSection = builder.Configuration.GetSection("Jwt");
var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]);
var tokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidIssuer = jwtSection["Issuer"],
    ValidateAudience = true,
    ValidAudience = jwtSection["Audience"],
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
    ValidateLifetime = Convert.ToBoolean(jwtSection["ValidateLifetime"] ?? "true")
};

builder.Services.AddSingleton(tokenValidationParameters);

builder.Services.AddScoped<CookieService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<AccessTokenService>();
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AuthApi:BaseUrl"]);
});

builder.Services.AddHttpClient("ResourceClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ResourceApi:BaseUrl"]);
});


builder.Services.AddScoped<AuthService>();

builder.Services.AddAuthentication()
    .AddScheme<CustomOption, JwtAuthenticationHandler>(
    "JwtAuth", options => { }
    );
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<ResourceService>();
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();
