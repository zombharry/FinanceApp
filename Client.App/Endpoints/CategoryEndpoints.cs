using System.Net.Http.Json;

namespace Client.App.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/category")
            .RequireAuthorization();

        group.MapGet("/getall", GetAllAsync);

        return app;
    }

    private static async Task<IResult> GetAllAsync(IHttpClientFactory httpClientFactory)
    {
        var client = httpClientFactory.CreateClient("ResourceClient"); // FONTOS: ez a külső Product Api-ra mutat
        var response = await client.GetAsync("api/category/getall");
        return await ForwardAsync(response);
    }

    private static async Task<IResult> ForwardAsync(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        return Results.Content(
            content,
            contentType: "application/json",
            statusCode: (int)response.StatusCode);
    }
}