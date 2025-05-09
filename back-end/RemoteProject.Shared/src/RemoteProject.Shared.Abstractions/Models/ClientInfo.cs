namespace RemoteProject.Shared.Abstractions.Models;

public class ClientInfo
{
    public string? ClientId { get; set; }
    public string? ClientVersion { get; set; }

    public string? ClientPlatform { get; set; }

    public string? DeviceId { get; set; }

    public ClientInfo()
    {
    }

    public ClientInfo(
        string? clientId
    )
    {
        ClientId = clientId;
    }

    public override string ToString()
    {
        return ClientId ?? string.Empty;
    }

    protected bool Equals(ClientInfo other) =>
        ClientId == other.ClientId &&
        ClientVersion == other.ClientVersion &&
        ClientPlatform == other.ClientPlatform &&
        DeviceId == other.DeviceId;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ClientInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            ClientId ?? string.Empty,
            ClientVersion ?? string.Empty,
            ClientPlatform ?? string.Empty,
            DeviceId ?? string.Empty
        );
    }

    public static implicit operator string?(ClientInfo client) => client.ClientId;

    public static implicit operator ClientInfo?(string? clientId) => clientId is null ? null : new ClientInfo(clientId);
}
