using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownNotFoundException : KnownException
{
    public const string ErrorCode = "NotFound";

    public KnownNotFoundException(string message)
        : this(ErrorCode, message)
    {
    }

    public KnownNotFoundException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
