using System.Runtime.Serialization;

namespace RemoteProject.Shared.Abstractions.Exceptions;

public class KnownException : Exception
{
    public string Code { get; set; }

    public int StatusCode { get; set; } = 500;


    public KnownException(string code, string fallbackMessage)
        : base(fallbackMessage)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }

    public KnownException(string code, string fallbackMessage, Exception innerException)
        : base(fallbackMessage, innerException)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
    }

    protected KnownException(SerializationInfo info, StreamingContext context) :
        base(info, context)
    {
        Code = info.GetString(nameof(Code));
    }
}
