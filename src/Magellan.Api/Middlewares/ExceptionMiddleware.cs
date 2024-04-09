using Magellan.Domain.Exceptions;

namespace Magellan.Api.Middlewares;

using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {

        switch (exception)
        {
            case UserNotExistsException :
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return context.Response.WriteAsync("User not found");
            case ConversationNotExistsException :
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return context.Response.WriteAsync("Conversation not found");
            case ModelNotExistsException :
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return context.Response.WriteAsync("Model not found");
            case WrongFileFormatException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return context.Response.WriteAsync("Wrong file format");
            default:
                throw exception;
        }
    }
}