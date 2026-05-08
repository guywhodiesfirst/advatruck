namespace API.Middlewares;

using System.Net;
using System.Text.Json;
using Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        logger.LogError(exception, "An exception has occurred: {Exception}", exception);

        var response = context.Response;
        response.ContentType = "application/json";

        var statusCode = HttpStatusCode.InternalServerError;
        var message = "Internal Server Error. Please try again later.";

        if (exception is TmsException tmsEx)
        {
            statusCode = tmsEx.StatusCode ?? HttpStatusCode.BadRequest;
            message = tmsEx.Message;
        }

        response.StatusCode = (int)statusCode;

        var result = JsonSerializer.Serialize(new
        {
            error = message,
            status = response.StatusCode,
        });

        await response.WriteAsync(result);
    }
}