using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Models;
using Oqtane.Shared;

namespace Oqtane.Services
{
    public class ThemeService : ServiceBase, IThemeService
    {
        private readonly SiteState _siteState;

        public ThemeService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string ApiUrl => CreateApiUrl("Theme", _siteState.Alias);

        public async Task<List<Theme>> GetThemesAsync()
        {
            List<Theme> themes = await GetJsonAsync<List<Theme>>(ApiUrl);
            return themes.OrderBy(item => item.Name).ToList();
        }

        public List<ThemeControl> GetThemeControls(List<Theme> themes)
        {
            return themes.SelectMany(item => item.Themes).ToList();
        }

        //[Obsolete("This method is deprecated.", false)]
        public List<ThemeControl> GetLayoutControls(List<Theme> themes, string themeName)
        {
            return null;
        }

        public List<ThemeControl> GetContainerControls(List<Theme> themes, string themeName)
        {
            return themes.Where(item => Utilities.GetTypeName(themeName).StartsWith(Utilities.GetTypeName(item.ThemeName)))
                .SelectMany(item => item.Containers).ToList();
        }

        public async Task InstallThemesAsync()
        {
            await GetJsonAsync<List<string>>($"{ApiUrl}/install");
        }

        public async Task DeleteThemeAsync(string themeName)
        {
            await DeleteAsync($"{ApiUrl}/{themeName}");
        }

        public async Task<Theme> CreateThemeAsync(Theme theme)
        {
            return await PostJsonAsync($"{ApiUrl}", theme);
        }

        public async Task<List<string>> GetThemeTemplatesAsync()
        {
            List<string> templates = await GetJsonAsync<List<string>>($"{ApiUrl}/templates");
            return templates;
        }
    }
}
