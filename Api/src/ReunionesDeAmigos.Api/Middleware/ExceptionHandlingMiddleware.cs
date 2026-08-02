using Microsoft.AspNetCore.Mvc;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Api.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            await EscribirRespuestaAsync(context, exception);
        }
    }

    private async Task EscribirRespuestaAsync(
        HttpContext context,
        Exception exception)
    {
        var (statusCode, title, detail) = exception switch
        {
            ApplicationValidationException => (
                StatusCodes.Status400BadRequest,
                "Solicitud inválida",
                exception.Message),
            DomainException => (
                StatusCodes.Status400BadRequest,
                "Regla de negocio incumplida",
                exception.Message),
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Recurso no encontrado",
                exception.Message),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Conflicto",
                exception.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Error interno",
                "Ocurrió un error inesperado.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(
                exception,
                "Error inesperado al procesar {Method} {Path}.",
                context.Request.Method,
                context.Request.Path);
        }

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(
            problemDetails,
            context.RequestAborted);
    }
}
