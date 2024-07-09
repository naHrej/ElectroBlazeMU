using Newtonsoft.Json;

namespace BlazorApp1.Settings;

public class Settings<T> : ISettings<T> where T : class
{
    Dictionary<string,T>? settings = new Dictionary<string,T>();

    public T? get(string key)
    {
        if (settings == null)
        {
            var filename = typeof(T).Name + ".json";
            if (File.Exists(filename))
            {
                var json = File.ReadAllText(filename);
                settings = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            }
            else
            {
                settings = new Dictionary<string, T>();
            }
        }

        if (settings.TryGetValue(key, out T value))
        {
            return value;
        }

        return null;
    }

    public void set(string key, T value)
    {
        // Implementation to set the setting value by key
        throw new NotImplementedException();
    }
}