using RemoteProject.Shared.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace RemoteProject.Shared.AspNet.Helpers;

public static class ControllerExtensions
{
    private const string JwtClientIdClaim = "client_id";
    private const string JwtClientVersionClaim = "client_version";

    public static ClientInfo OrDefault(
        this ClientInfo? client,
        HttpContext context
    )
    {
        if (client is null)
        {
            return new ClientInfo
            {
                ClientId = context.User.FindFirst(JwtClientIdClaim)?.Value,
                ClientVersion = context.User.FindFirst(JwtClientVersionClaim)?.Value,
                DeviceId = null,
            };
        }

        if (string.IsNullOrWhiteSpace(client.ClientId))
        {
            client.ClientId = context.User.FindFirst(JwtClientIdClaim)?.Value;
        }

        if (string.IsNullOrWhiteSpace(client.ClientVersion))
        {
            client.ClientVersion = context.User.FindFirst(JwtClientVersionClaim)?.Value;
        }

        return client;
    }
}
