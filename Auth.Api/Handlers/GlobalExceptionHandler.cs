using Auth.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = new ProblemDetails();

        switch (exception)
        {
            case ValidationFailedException validation:
                problem.Status = StatusCodes.Status400BadRequest;
                problem.Title = "Validation failed";
                problem.Detail = "One or more validation errors occured";

                problem.Extensions["errors"] =
                    validation.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage));
                break;

            case UnauthorizedException:
                problem.Status = StatusCodes.Status401Unauthorized;
                problem.Title = "Unauthorized";
                problem.Detail = exception.Message;
                break;

            case NotFoundException:
                problem.Status = StatusCodes.Status404NotFound;
                problem.Title = "Resource not found";
                problem.Detail = exception.Message;
                break;

            default:
                problem.Status = StatusCodes.Status500InternalServerError;
                problem.Title = "Internal Server Error";
                problem.Detail = "Unexpected error.";
                break;
        }

        httpContext.Response.StatusCode = problem.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
