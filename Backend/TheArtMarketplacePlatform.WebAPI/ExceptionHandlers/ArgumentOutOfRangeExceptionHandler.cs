using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TheArtMarketplacePlatform.WebAPI.ExceptionHandlers
{
    public class ArgumentOutOfRangeExceptionHandler(ILogger<ArgumentOutOfRangeExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not ArgumentOutOfRangeException ex) return false;

            logger.LogError("Argument out of range exception : {Exception}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Status = 400,
                Type = "https://httpstatuses.com/400",
                Title = "Invalid argument",
                Detail = exception.Message // Masquer en prod si besoin
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails), cancellationToken);

            return true;
        }
    }
}