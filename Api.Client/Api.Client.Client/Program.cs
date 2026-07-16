using Api.Client.Client.Authentication;
using Api.Client.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Mythetech.LocalStorage;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7268/");
});

builder.Services.AddHttpClient<ItemService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7070");
})
.AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddTransient<AuthorizationHandler>();

builder.Services.AddScoped<ITokenStore, TokenStore>();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<AuthenticationStateProvider,
    JwtAuthenticationStateProvider>();

await builder.Build().RunAsync();
