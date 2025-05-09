using RemoteProject.Shared.Abstractions.Exceptions;

namespace RemoteProject.Shared.AspNet.Helpers;

internal static class InternalHelpers
{
    public static Uri BuildServiceUri(
        string? host,
        int? port
    )
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            host = "localhost";
        }

        if (!host.StartsWith("http"))
        {
            host = "http://" + host;
        }

        try
        {
            var builder = new UriBuilder(host);

            if (port.HasValue)
            {
                builder.Port = port.Value;
            }

            return builder.Uri;
        }
        catch (Exception ex)
        {
            throw new KnownException("InvalidUrl", $"The given url '{ex}' is invalid.", ex);
        }
    }
}
