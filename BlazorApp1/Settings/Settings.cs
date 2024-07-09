using Newtonsoft.Json;

namespace BlazorApp1.Settings;

public class Settings<T> : ISettings<T> where T : class
{
    Dictionary<string,T>? settings = new Dictionary<string,T>();

    public T? Get(string key)
    {
        if (settings == null)
        {
            settings = Load();
        }

        return settings.GetValueOrDefault(key);
    }

    private Dictionary<string, T> Load()
    {
        var filename = typeof(T).Name + ".json";
        if (File.Exists(filename))
        {
            var json = File.ReadAllText(filename);
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(json) ?? new Dictionary<string, T>();
        }
        else
        {
            return new Dictionary<string, T>();
        }

    }

    public void Set(string key, T value)
    {
        if (settings == null)
        {
            settings = Load();
        }

        settings[key] = value;

        var filename = typeof(T).Name + ".json";
        var json = JsonConvert.SerializeObject(settings);
        File.WriteAllText(filename, json);
    }
}