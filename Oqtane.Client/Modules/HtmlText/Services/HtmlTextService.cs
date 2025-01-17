using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Oqtane.Modules.HtmlText.Models;
using Oqtane.Services;
using Oqtane.Shared;

namespace Oqtane.Modules.HtmlText.Services
{
    public class HtmlTextService : ServiceBase, IHtmlTextService, IService
    {        
        private readonly SiteState _siteState;

        public HtmlTextService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string ApiUrl => CreateApiUrl("HtmlText", _siteState.Alias);

        public async Task<Models.HtmlText> GetHtmlTextAsync(int moduleId)
        {
            var htmltext = await GetJsonAsync<List<Models.HtmlText>>(CreateAuthorizationPolicyUrl($"{ApiUrl}/{moduleId}", moduleId));
            return htmltext.FirstOrDefault();
        }

        public async Task AddHtmlTextAsync(Models.HtmlText htmlText)
        {
            await PostJsonAsync(CreateAuthorizationPolicyUrl($"{ApiUrl}", htmlText.ModuleId), htmlText);
        }

        public async Task UpdateHtmlTextAsync(Models.HtmlText htmlText)
        {
            await PutJsonAsync(CreateAuthorizationPolicyUrl($"{ApiUrl}/{htmlText.HtmlTextId}", htmlText.ModuleId), htmlText);
        }

        public async Task DeleteHtmlTextAsync(int moduleId)
        {
            await DeleteAsync(CreateAuthorizationPolicyUrl($"{ApiUrl}/{moduleId}", moduleId));
        }
    }
}
