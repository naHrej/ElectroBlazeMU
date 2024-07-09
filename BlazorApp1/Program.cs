using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorApp1.Components;
using BlazorApp1.Settings;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Server.Circuits;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.ResponseCompression;
using App = BlazorApp1.Components.App;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddMudPopoverService();


builder.Services.AddSingleton<CircuitHandler, CustomCircuitHandler>();

        // Add Electron
        builder.WebHost.UseElectron(args);
        builder.Services.AddElectron();
        builder.Services.AddBlazoredLocalStorage(config =>
        {
            config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
            config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
            config.JsonSerializerOptions.WriteIndented = false;
        });
        builder.Services.AddScoped<ISettings, Settings>();

        // AllowAny Cors
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAny", cors =>
            {
                cors.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        // Allow WebFont download from any origin
        builder.Services.AddResponseCompression(options =>
        {
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "font/woff2" });
        });

        

var app = builder.Build();



    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    if(HybridSupport.IsElectronActive)
    {
        var options = new BrowserWindowOptions
        {
            WebPreferences = new WebPreferences
            {
                WebSecurity = false,
                NodeIntegration = true
            }
        };
        var window = await Electron.WindowManager.CreateWindowAsync(options);  
        window.OnClosed += () => {  
            Electron.App.Quit();  
        }; 
    }
    


//app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.UseCors("AllowAny");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
{
    Console.WriteLine($"Unhandled exception: {eventArgs.ExceptionObject}");
};

app.Run();