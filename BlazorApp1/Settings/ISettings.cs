namespace BlazorApp1.Settings;

public interface ISettings<T> where T : class
{
    T? Get(string key);
    void Set(string key, T value);

}