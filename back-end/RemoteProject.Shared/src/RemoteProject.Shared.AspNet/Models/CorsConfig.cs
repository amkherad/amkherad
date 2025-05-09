namespace RemoteProject.Shared.AspNet.Models;

public class CorsConfig
{
    public bool? IsEnabled { get; set; }

    public string? PolicyName { get; set; }

    public bool? IsDefaultPolicy { get; set; }

    public bool? AllowAnyOrigin { get; set; }
    public bool? AllowAnyHeader { get; set; }
    public bool? AllowAnyMethod { get; set; }

    public bool? AllowCredentials { get; set; }

    public string[]? Origins { get; set; }
    public string[]? Headers { get; set; }
    public string[]? Methods { get; set; }
    public string[]? ExposedHeaders { get; set; }
}
