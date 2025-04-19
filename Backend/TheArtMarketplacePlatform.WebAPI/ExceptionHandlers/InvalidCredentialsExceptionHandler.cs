using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TheArtMarketplacePlatform.BusinessLayer.Exceptions;

namespace TheArtMarketplacePlatform.WebAPI.ExceptionHandlers
{
    public class InvalidCredentialsExceptionHandler(ILogger<InvalidCredentialsExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is not InvalidCredentialsException ex) return false;

            logger.LogError("Invalid credentials exception : {Exception}", ex.Message);

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Type = "https://httpstatuses.com/400",
                Title = "Wrong credentials provided",
                Detail = exception.Message // Masquer en prod si besoin
            };

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = problemDetails.Status.Value;

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(problemDetails), cancellationToken);

            return true;
        }
    }
}