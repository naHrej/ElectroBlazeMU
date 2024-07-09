namespace BlazorApp1.Settings;

public class Site
{
    public string Name { get; set; } = "Default";
    public Uri? Host { get; set; } = new("localhost:1701");


    public string? ConnectionString { get; set; } = "DefaultConnection";
    public bool AutoConnect { get; set; } = false;
    public bool AutoReconnect { get; set; } = false;
    public bool AnsiEnabled { get; set; } = true;
    public bool HtmlEnabled { get; set; } = true;
    public bool LoggingEnabled { get; set; } = false;
    public bool DebugEnabled { get; set; } = false;
}