namespace RemoteProject.Shared.AspNet.Models;

public class AppServiceConfig
{
    public bool IsEnabled { get; set; }

    public string ServiceId { get; set; } = default!;
    public string ServiceName { get; set; } = default!;
    public string ServiceVersion { get; set; } = default!;
}
