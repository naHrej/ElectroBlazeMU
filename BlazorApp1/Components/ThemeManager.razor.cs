using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.ThemeManager;

namespace BlazorApp1.Components;

public partial class ThemeManager : ComponentBase
{
    private ThemeManagerTheme _themeManager = new ThemeManagerTheme();
    public bool _themeManagerOpen = false;
    public bool _drawerOpen = false;
    
    

    void OpenThemeManager(bool value)
    {
        _themeManagerOpen = value;
    }

    void UpdateTheme(ThemeManagerTheme value)
    {
        _themeManager = value;
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        StateHasChanged();
    }
    
    
}