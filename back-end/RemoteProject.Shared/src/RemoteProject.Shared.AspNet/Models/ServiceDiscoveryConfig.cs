namespace RemoteProject.Shared.AspNet.Models;

public class ServiceDiscoveryConfig : AppServiceConfig
{
    public string? DiscoveryServiceHost { get; set; }
    public int DiscoveryServicePort { get; set; }

    public string? ThisServiceHost { get; set; }
    public int ThisServicePort { get; set; }

    public string? ThisServiceHealthCheckHost { get; set; }
    public string? ThisServiceHealthCheckPath { get; set; }
    public int ThisServiceHealthCheckPort { get; set; }

    public string[]? Tags { get; set; }

    public TimeSpan? RegistrationTimeout { get; set; }
    public TimeSpan? RegistrationInterval { get; set; }
    public TimeSpan? RegistrationDeregisterCriticalServiceAfter { get; set; }
    public bool? RegistrationTLSSkipVerify { get; set; }
    
    public TimeSpan ReloadInterval { get; set; }
}
