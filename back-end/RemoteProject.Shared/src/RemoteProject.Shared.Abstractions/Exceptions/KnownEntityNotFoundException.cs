using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownEntityNotFoundException : KnownNotFoundException
{
    public string EntityName { get; set; }

    public string? Id { get; set; }

    public KnownEntityNotFoundException(string entityName)
        : this(ErrorCode, $"A '{entityName}' was not found.")
    {
        EntityName = entityName;
    }

    public KnownEntityNotFoundException(string entityName, object? id)
        : this(ErrorCode, $"A '{entityName}' with id of '{id ?? "null"}' was not found.")
    {
        EntityName = entityName;
        Id = id?.ToString();
    }

    public KnownEntityNotFoundException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownEntityNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
