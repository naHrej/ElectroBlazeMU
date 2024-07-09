using System.Runtime.InteropServices;
using BlazorApp1.Settings;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Components;

public partial class SiteManager : ComponentBase
{
    [Inject] private ISettings Settings { get; set; } = null!;
    private List<Site> _sites;
    private List<Site> Sites { get => _sites; set { _sites = value; SaveSites(); } }
    private bool isEditing = false;
    private Site? editingSite = null;

    protected override async Task OnAfterRenderAsync(bool firstRender) 
    {
        // call base method
        await base.OnAfterRenderAsync(firstRender);
        if(firstRender)
        {
            var sites = await Settings.Get("sites");
            Sites = sites.Select(s => (Site)s.value).ToList();
        }
    }
    
    private async Task AddSite()
    {
        Sites.Add(new Site());
        SaveSites();
        await InvokeAsync(StateHasChanged);
        
    }
    
    private async Task RemoveSite(Site site)
    {
        Sites.Remove(site);
        await InvokeAsync(StateHasChanged);
    }
    
    private void SaveSites()
    {
        Settings.Set("sites", Sites);
    }

    private async Task EditSite(Site site)
    {
        isEditing = true;
        editingSite = site;
        await InvokeAsync(StateHasChanged);
        
    }
    
    private async Task DeleteSite(Site site)
    {
        Sites.Remove(site);
        SaveSites();
        await InvokeAsync(StateHasChanged);
    }
    
    
}