using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cbd.Api;

/// <summary>
/// Globální handler zpracovává chyby z kontroleru a určuje formát vrácené chybové odpovědi. 
/// V tomto případě vrací standardní chybovou odpověď ve formátu RFC 7807 Problem Details, která obsahuje pouze obecné informace o chybě, ne kritické detaily.
/// To umožňuje čistý kód v controlleru bez try/catch v každé akci
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) 
    : IExceptionHandler
{

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Exception: {Message}", exception.Message);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = "application/problem+json";

        // Vrácení standardní chybové odpovědi (RFC 7807 Problem Details)
        // Nechceme klientovi posílat detaily o chybě kvůli bezpečnosti, stačí obecné info
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Detail = "Na serveru došlo k neočekávané chybě."
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // True = že jsme chybu zpracovali a nemá se předávat dalším handlerům
        return true;
    }
}