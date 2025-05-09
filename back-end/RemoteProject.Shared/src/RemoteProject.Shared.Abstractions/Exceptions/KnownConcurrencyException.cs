using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownConcurrencyException : KnownException
{
    public const string ErrorCode = "Concurrency";

    public KnownConcurrencyException()
        : this(ErrorCode, ErrorCode)
    {
    }

    public KnownConcurrencyException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownConcurrencyException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
