using System.Net;
using RemoteProject.Shared.Abstractions.Exceptions;
using RemoteProject.Shared.Abstractions.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace RemoteProject.Shared.AspNet;

public static class ExceptionHandlerConfigurations
{
    public static IApplicationBuilder UseAppExceptionHandler(
        this IApplicationBuilder applicationBuilder,
        Func<Exception, ErrorDto?>? exceptionHandler
    )
    {
        return applicationBuilder.UseExceptionHandler(
            config => config.Run(ctx => ExceptionHandler(ctx, exceptionHandler))
        );
    }

    public static IApplicationBuilder UseAppExceptionHandler(
        this IApplicationBuilder applicationBuilder
    )
    {
        return applicationBuilder.UseExceptionHandler(
            config => config.Run(ctx => ExceptionHandler(ctx, null))
        );
    }

    private static async Task ExceptionHandler(
        HttpContext context,
        Func<Exception, ErrorDto?>? exceptionHandler
    )
    {
        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        var exception = exceptionHandlerFeature?.Error;

        ErrorDto? response = null;
        if (exceptionHandler is not null && exception is not null)
        {
            response = exceptionHandler(exception);
        }

        if (response is null)
        {
            response = GetErrorFromException(exception);
        }

        context.Response.StatusCode = response.StatusCode;
        await context.Response.WriteAsJsonAsync(response);

        await context.Response.CompleteAsync();
    }

    public static ErrorDto GetErrorFromException(
        Exception? exception
    )
    {
        var result = new ErrorDto();

        switch (exception)
        {
            case KnownNotFoundException notFound:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "ApiNotFoundException",
                        Code = notFound.Code,
                        Message = notFound.Message,
                    }
                };
                result.StatusCode = (int) HttpStatusCode.NotFound;
                break;
            case KnownValidationException validation:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "ApiValidationException",
                        Code = validation.Code,
                        Message = validation.Message,
                    }
                };
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            case System.ComponentModel.DataAnnotations.ValidationException validation:
                result.Exceptions = validation.Data.OfType<KeyValuePair<string, object>>().Select(ex =>
                        new ErrorEntryDto
                        {
                            Class = "ApiValidationException",
                            Code = "Validation",
                            Message = validation.Message,
                        }
                    )
                    .ToArray();
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            case FormatException formatException:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "ApiValidationException",
                        Code = "InvalidFormat",
                        Message = formatException.Message,
                    }
                };
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                break;
            case KnownConcurrencyException concurrency:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "MateConcurrencyException",
                        Code = concurrency.Code,
                        Message = concurrency.Message,
                    }
                };
                result.StatusCode = (int) HttpStatusCode.Conflict;
                break;
            case KnownException appException:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "ApiNotFoundException",
                        Code = appException.Code,
                        Message = appException.Message,
                    }
                };
                result.StatusCode = appException.StatusCode;
                break;
            case NotImplementedException _:
            case NotSupportedException _:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "ApiNotFoundException",
                        Code = "NotFound",
                        Message = "Resource was not found.",
                    }
                };
                result.StatusCode = (int) HttpStatusCode.NotFound;
                break;
            case ArgumentNullException _:
            case ArgumentOutOfRangeException _:
            case ArgumentException _:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "ApiValidationException",
                        Code = "Validation",
                        Message = "Invalid parameter.",
                    }
                };
                result.StatusCode = (int) HttpStatusCode.BadRequest;
                break;

            default:
                result.Exceptions = new[]
                {
                    new ErrorEntryDto
                    {
                        Class = "Exception",
                        Code = "Exception",
                        Message = exception.Message,
                    }
                };
                result.StatusCode = 500;
                break;
        }

        return result;
    }
}
