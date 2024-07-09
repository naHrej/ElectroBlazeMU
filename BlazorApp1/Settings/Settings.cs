using Blazored.LocalStorage;
using ElectronNET.API;
using Newtonsoft.Json;

namespace BlazorApp1.Settings;

public class Settings : ISettings
{

    private List<(string key, Type type, object value)>? _settings;
    private readonly ILocalStorageService _localStorage;
    private static bool IsElectron => ElectronNET.API.HybridSupport.IsElectronActive;

    private readonly string _filePath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "settings.json");

    public Settings(ILocalStorageService localStorageService) => _localStorage = localStorageService;

    private async Task<List<(string key, Type type, object value)>> Load()
    {
        try
        {
            if (IsElectron)
            {
                if (File.Exists(_filePath))
                {
                    var json = await File.ReadAllTextAsync(_filePath);
                    return JsonConvert.DeserializeObject<List<(string key, Type type, object value)>>(json) ??
                           new List<(string key, Type type, object value)>();
                }
            }
            else
            {
                var settings =
                    await _localStorage.GetItemAsync<List<(string key, Type type, object value)>>("userSettings");
                return settings ?? new List<(string key, Type type, object value)>();
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"An error occurred while loading settings: {ex.Message}");
            return new List<(string key, Type type, object value)>();
        }

        return new List<(string key, Type type, object value)>();
    }

    private async Task Save()
    {
        if (IsElectron)
        {
            var json = JsonConvert.SerializeObject(_settings);
            await File.WriteAllTextAsync(_filePath, json);
        }
        else
        {
            // save to localStorage
            await _localStorage.SetItemAsync("userSettings", _settings);
        }
    }

    public async Task<List<(string key, Type type, object value)>> Get(string key)
    {
        if (_settings == null)
        {
            _settings = await Load();
        }

        return _settings.Where(s => s.key == key).ToList();
    }

    public async Task Set(string key, object? value)
    {
        if (_settings == null)
        {
            _settings = await Load();
        }

        (string key, Type type, object? value) setting = _settings.FirstOrDefault(s => s.key == key);
        if (setting.key == null)
        {
            _settings.Add((key, value.GetType(), value));
        }
        else
        {
            setting.value = value;
        }

        await Save();
    }
}