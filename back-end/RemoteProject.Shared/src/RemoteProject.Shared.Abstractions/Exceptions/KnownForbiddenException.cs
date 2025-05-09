using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownForbiddenException : KnownException
{
    public const string ErrorCode = "Forbidden";

    public KnownForbiddenException()
        : this(ErrorCode, ErrorCode)
    {
    }

    public KnownForbiddenException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownForbiddenException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
