using System.Collections.Generic;
using System.Threading.Tasks;

namespace azure_friday.web.services
{
    public interface IAzureFridayDB
    {
        Task<List<Episode>> GetVideos();
        Task<List<Episode>> PopulateVideosCache();
        public bool PurgeCache();
    }
}