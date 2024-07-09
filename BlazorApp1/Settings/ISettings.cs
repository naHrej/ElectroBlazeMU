namespace BlazorApp1.Settings;

public interface ISettings
{
    public Task<List<(string key, Type type, object value)>> Get(string key);
    public Task Set(string key, object? value);

}