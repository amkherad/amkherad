using RemoteProject.Shared.Abstractions.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RemoteProject.Shared.AspNet.Helpers;

public static class ModelValidationExtensions
{
    public static void ThrowIfInvalid(
        this ModelStateDictionary modelState
    )
    {
        if (!modelState.IsValid)
        {
            if (!modelState.Values.TryGetNonEnumeratedCount(out var len))
            {
                len = 1;
            }

            if (len == 1)
            {
                throw new KnownValidationException(
                    modelState.Values.FirstOrDefault()?
                        .Errors.FirstOrDefault()?
                        .ErrorMessage ?? "InvalidRequest",
                    ""
                );
            }

            throw new AggregateException(
                modelState.Values.Select(ex => new KnownValidationException(
                    "Validation",
                    ex.Errors.FirstOrDefault()?.ErrorMessage ?? "InvalidRequest"
                ))
            );
        }
    }

    public static void ThrowIfInvalid(
        this ModelStateDictionary modelState,
        object model
    )
    {
        ThrowIfInvalid(modelState);

        if (model is null)
        {
            throw new KnownValidationException("ModelIsNull", "Null model was encountered.");
        }
    }

    public static void ThrowIfInvalid(
        this Guid id
    )
    {
        if (id == Guid.Empty)
        {
            throw new KnownValidationException("InvalidId", "Empty id was encountered.");
        }
    }
}
