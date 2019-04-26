using System.Collections.Generic;

namespace azure_friday.core.services
{
    public interface IAzureFridayDB
    {
        List<Video> GetVideos();
    }
}