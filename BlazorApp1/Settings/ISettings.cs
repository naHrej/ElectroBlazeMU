namespace BlazorApp1.Settings;

public interface ISettings<T> where T : class
{
    T get(string key);
    void set(string key, T value);

}