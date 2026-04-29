using System.Net;
using System.Text.Json;

namespace SmartHome.Api.Middleware;

public class GlobalErrorHandling
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandling> _logger;

    public GlobalErrorHandling(RequestDelegate next, ILogger<GlobalErrorHandling> logger)
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
        // Catch specific exceptions to return appropriate status codes and messages
        // 404 Not Found for missing resources
        catch (KeyNotFoundException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        // 400 Bad Request for invalid input or operations
        catch (ArgumentException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        // 501 Not Implemented for features that are not yet implemented
        catch (NotImplementedException ex)
        {
            await WriteErrorResponse(context, HttpStatusCode.NotImplemented, ex.Message);
        }
        // Catch any other unhandled exceptions to return a generic 500 Internal Server Error
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = context.Response.StatusCode,
            message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}