namespace RemoteProject.Shared.Abstractions.Models;

public class ErrorDto
{
    public int StatusCode { get; set; } = 500;
    public bool Success => StatusCode >= 100 && StatusCode <= 399;

    // Namings are done this way to keep compatibility with the legacy app.
    public ErrorEntryDto[] Exceptions { get; set; }
}

public class ErrorEntryDto
{
    public string Class { get; set; }

    public string Code { get; set; }

    public string Message { get; set; }
}
