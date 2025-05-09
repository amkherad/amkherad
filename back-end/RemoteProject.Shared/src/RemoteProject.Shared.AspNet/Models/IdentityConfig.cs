namespace RemoteProject.Shared.AspNet.Models;

public class IdentityConfig
{
    public string AuthorityUrl { get; set; } = default!;
    public bool? RequireHttps { get; set; }

    public bool? SaveToken { get; set; }
    public bool? EnableCaching { get; set; }
    public TimeSpan? CacheDuration { get; set; }

    public string? Audience { get; set; }

    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
}
