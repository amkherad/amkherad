using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownUnauthorizedException : KnownException
{
    public const string ErrorCode = "Unauthorized";

    public KnownUnauthorizedException()
        : this(ErrorCode, ErrorCode)
    {
    }

    public KnownUnauthorizedException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownUnauthorizedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
