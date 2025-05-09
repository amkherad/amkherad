using System.Net;
using RemoteProject.Shared.Abstractions.Exceptions;
using RemoteProject.Shared.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace RemoteProject.Shared.Data.EntityFramework;

public static class EntityFrameworkExceptionHandler
{
    public static Func<Exception, ErrorDto?> MapKnownExceptions() => MapKnownExceptions(null);

    public static Func<Exception, ErrorDto?> MapKnownExceptions(
        Func<Exception, ErrorDto?>? next
    )
    {
        return ex =>
        {
            switch (ex)
            {
                case DbUpdateConcurrencyException concurrency:
                    return new ErrorDto
                    {
                        Exceptions = new[]
                        {
                            new ErrorEntryDto
                            {
                                Class = nameof(KnownConcurrencyException),
                                Code = KnownConcurrencyException.ErrorCode,

                                // concurrency.Message contains sensitive information, DO NOT return it.
                                Message = KnownConcurrencyException.ErrorCode,
                            }
                        },
                        StatusCode = (int) HttpStatusCode.Conflict
                    };
            }

            return next?.Invoke(ex);
        };
    }
}
