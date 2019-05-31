using System.Collections.Generic;
using System.Threading.Tasks;

namespace azure_friday.core.services
{
    public interface IAzureFridayDB
    {
        Task<Response> GetVideos();
        Task<Response> PopulateVideosCache();
    }
}