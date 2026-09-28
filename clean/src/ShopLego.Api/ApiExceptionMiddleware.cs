using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            var status = ex switch { NotFoundException => 404, ConflictException => 409, BusinessRuleException => 400, _ => 500 };
            if (status == 500) logger.LogError(ex, "Unhandled request error"); context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Title = status == 500 ? "Unexpected server error" : ex.Message, Detail = status == 500 ? null : ex.Message, Instance = context.Request.Path });
        }
    }
}
