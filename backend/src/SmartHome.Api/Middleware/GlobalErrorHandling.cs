using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using SmartHome.Domain.Exceptions;

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
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Resource not found.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.NotFound,
                "about:blank",
                "Not Found",
                "The requested resource could not be found.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogWarning(ex, "Invalid range.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.BadRequest,
                "about:blank",
                "Invalid Request Range",
                "One or more values were outside the allowed range.");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.BadRequest,
                "about:blank",
                "Bad Request",
                "The request contains invalid input.");
        }
        catch (DeviceConflictException ex)
        {
            _logger.LogWarning(ex, "Device conflict.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.Conflict,
                "about:blank",
                "Conflict",
                "The requested device conflicts with an existing device.");
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.BadRequest,
                "about:blank",
                "Invalid Operation",
                "The requested operation could not be completed.");
        }
        catch (NotImplementedException ex)
        {
            _logger.LogWarning(ex, "Feature not implemented.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.NotImplemented,
                "about:blank",
                "Not Implemented",
                "This feature is not implemented.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred.");
            await WriteErrorResponse(
                context,
                HttpStatusCode.InternalServerError,
                "about:blank",
                "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        HttpStatusCode statusCode,
        string type,
        string title,
        string detail)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problemDetails = new ProblemDetails
        {
            Type = type,
            Title = title,
            Detail = detail,
            Status = context.Response.StatusCode,
            Instance = context.Request.Path
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}