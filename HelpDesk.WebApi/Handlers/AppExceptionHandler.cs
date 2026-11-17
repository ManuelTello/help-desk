using HelpDesk.Service.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.WebApi.Handlers;

public class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception ex, CancellationToken cancellationToken)
    {
        if (ex is HelpDeskException applicationException)
        {
            int code = applicationException.Code switch
            {
                "NOTFOUND" => StatusCodes.Status404NotFound,
                "CONFLICT" => StatusCodes.Status409Conflict,
                "INVALID" => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status400BadRequest
            };

            httpContext.Response.StatusCode = code;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = code,
                Extensions = { { "message", applicationException.Message }, { "Type", ex.GetType().Name } },
            }, cancellationToken);
            return true;
        }
        else if (ex is BadHttpRequestException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Extensions = { { "message", "One or more fields are not correctly formatted." }, { "Type", ex.GetType().Name } }
            }, cancellationToken);
            return true;
        }
        else
        {
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { { "message", "Internal server error." }, { "type", ex.GetType().Name } },
            }, cancellationToken);
            return true;
        }
    }
}
