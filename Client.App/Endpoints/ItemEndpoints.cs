using Client.App.DTO.AuthDtos;
using Client.App.DTO.ItemDtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Client.App.Endpoints;

public static class ItemEndpoints
{
    public static IEndpointRouteBuilder MapItemEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product")
            .RequireAuthorization();

        group.MapGet("/getallproduct", GetAllProductsAsync);
        group.MapGet("/getuserproduct", GetUserProductsAsync);
        group.MapGet("/getbyid", GetByIdAsync);
        group.MapPost("/create", CreateAsync);
        group.MapPost("/edit", EditAsync);
        group.MapDelete("/delete", DeleteAsync); 

        return app;
    }

    private static async Task<IResult> GetAllProductsAsync(HttpContext httpContext, IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient");

        await AttachTokenAsync(client, httpContext);
        var response = await client.GetAsync("api/product/getallproduct");
        return await ForwardAsync(response);
    }

    private static async Task<IResult> GetUserProductsAsync(
        Guid userId,
        HttpContext httpContext,
        IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient");
        await AttachTokenAsync(client, httpContext);

        var response = await client.GetAsync($"api/product/getuserproduct?userId={userId.ToString()}");
        return await ForwardAsync(response);
    }

    private static async Task<IResult> GetByIdAsync(
        Guid productId,
        HttpContext httpContext,
        IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient");
        await AttachTokenAsync(client, httpContext);
        var response = await client.GetAsync($"api/product/getbyid?productId={productId}");
        return await ForwardAsync(response);
    }

    private static async Task<IResult> CreateAsync(
        ProductCreateDto dto,
        HttpContext httpContext,
        IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient");
        await AttachTokenAsync(client, httpContext);
        var response = await client.PostAsJsonAsync("api/product/create", dto);
        return await ForwardAsync(response);
    }

    private static async Task<IResult> EditAsync(
        ProductEditDTO dto,
        ClaimsPrincipal user,
        HttpContext httpContext,
        IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient");
        await AttachTokenAsync(client, httpContext);

        var ownershipCheck = await CheckOwnershipAsync(client, Guid.Parse(dto.Id), user);
        if (ownershipCheck is not null) return ownershipCheck;

        var response = await client.PostAsJsonAsync("api/product/edit", dto);
        return await ForwardAsync(response);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        ClaimsPrincipal user,
        HttpContext httpContext,
        IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient");
        await AttachTokenAsync(client, httpContext);

        var ownershipCheck = await CheckOwnershipAsync(client, id, user);
        if (ownershipCheck is not null) return ownershipCheck;

        var response = await client.DeleteAsync($"api/product/delete?id={id}");
        return await ForwardAsync(response);
    }

    private static async Task<IResult?> CheckOwnershipAsync(
        HttpClient client, Guid productId, ClaimsPrincipal user)
    {
        var existingResponse = await client.GetAsync($"api/product/getbyid?productId={productId}");

        if (!existingResponse.IsSuccessStatusCode)
        {
            return Results.NotFound();
        }

        var existing = await existingResponse.Content.ReadFromJsonAsync<ProductGetDTO>();
        var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (existing is null || currentUserId is null || currentUserId != existing.OwnerId?.ToString())
        {
            return Results.Forbid();
        }

        return null;
    }

    private static async Task<IResult> ForwardAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return Results.Content(
            content,
            contentType: "application/json",
            statusCode: (int)response.StatusCode);
    }
    private static async Task AttachTokenAsync(HttpClient client, HttpContext httpContext)
    {
        var accessToken = await httpContext.GetTokenAsync(
            CookieAuthenticationDefaults.AuthenticationScheme, "access_token");

        if (!string.IsNullOrEmpty(accessToken))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
