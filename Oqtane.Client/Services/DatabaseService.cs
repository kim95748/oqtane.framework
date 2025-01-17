using Oqtane.Models;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;
using System.Collections.Generic;
using Oqtane.Shared;

namespace Oqtane.Services
{
    public class DatabaseService : ServiceBase, IDatabaseService
    {

        private readonly SiteState _siteState;

        public DatabaseService(HttpClient http, SiteState siteState) : base(http)
        {
            _siteState = siteState;
        }

        private string Apiurl => CreateApiUrl("Database", _siteState.Alias);

        public async Task<List<Database>> GetDatabasesAsync()
        {
            List<Database> databases = await GetJsonAsync<List<Database>>(Apiurl);
            return databases.OrderBy(item => item.FriendlyName).ToList();
        }
    }
}
