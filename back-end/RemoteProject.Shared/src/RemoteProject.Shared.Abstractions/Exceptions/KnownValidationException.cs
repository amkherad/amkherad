using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownValidationException : KnownException
{
    public const string ErrorCode = "Validation";

    public KnownValidationException()
        : this(ErrorCode, ErrorCode)
    {
    }

    public KnownValidationException(string code, string fallbackMessage)
        : base(code, fallbackMessage)
    {
    }

    protected KnownValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
