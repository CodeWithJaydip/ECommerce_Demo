using System.Net;
using System.Text.Json;
using ECommerce.Application.Common.Models;

namespace ECommerce.Api.Middleware;

/// <summary>
/// Global exception handler middleware
/// </summary>
public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. Request Path: {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var message = "An error occurred while processing your request.";
        var errors = new List<string>();

        switch (exception)
        {
            case InvalidOperationException:
                code = HttpStatusCode.BadRequest;
                message = exception.Message;
                errors.Add(exception.Message);
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                message = exception.Message;
                errors.Add(exception.Message);
                break;

            case ArgumentNullException argEx:
                code = HttpStatusCode.BadRequest;
                message = $"Required parameter is missing: {argEx.ParamName}";
                errors.Add(message);
                break;

            case ArgumentException argEx:
                code = HttpStatusCode.BadRequest;
                message = argEx.Message;
                errors.Add(argEx.Message);
                break;

            case KeyNotFoundException:
                code = HttpStatusCode.NotFound;
                message = "The requested resource was not found.";
                errors.Add(exception.Message);
                break;

            default:
                // For security, don't expose internal error details in production
                if (context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    message = exception.Message;
                    errors.Add(exception.Message);
                    if (exception.StackTrace != null)
                    {
                        errors.Add(exception.StackTrace);
                    }
                }
                else
                {
                    message = "An internal server error occurred. Please try again later.";
                }
                break;
        }

        var response = ApiResponse.ErrorResponse(message, errors);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}
