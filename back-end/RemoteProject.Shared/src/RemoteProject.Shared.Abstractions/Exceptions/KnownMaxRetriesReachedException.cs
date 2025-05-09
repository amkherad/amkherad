using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownMaxRetriesReachedException : KnownException
{
    public const string ErrorCode = "MaxRetriesReached";

    public KnownMaxRetriesReachedException()
        : this(ErrorCode, ErrorCode)
    {
    }

    public KnownMaxRetriesReachedException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownMaxRetriesReachedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}