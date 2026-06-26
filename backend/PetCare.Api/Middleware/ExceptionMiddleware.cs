using System.Net;
using System.Text.Json;
using PetCare.Api.Models;

namespace PetCare.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await HandleAppExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理的异常: {Path}", context.Request.Path);
            await HandleUnhandledExceptionAsync(context);
        }
    }

    private static async Task HandleAppExceptionAsync(HttpContext context, AppException ex)
    {
        context.Response.StatusCode = ex.StatusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        var json = JsonSerializer.Serialize(ApiResponse.Error(ex.StatusCode, ex.Message));
        await context.Response.WriteAsync(json);
    }

    private static async Task HandleUnhandledExceptionAsync(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json; charset=utf-8";
        var json = JsonSerializer.Serialize(ApiResponse.Error(500, "服务器内部错误，请稍后重试"));
        await context.Response.WriteAsync(json);
    }
}

public class AppException : Exception
{
    public int StatusCode { get; }
    public AppException(int statusCode, string message) : base(message) => StatusCode = statusCode;
    public AppException(string message) : this(400, message) { }
}
