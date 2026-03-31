using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.Extensions.Logging;

namespace azure_friday.core.services
{
    public class AzureFridayDB : IAzureFridayDB
    {
        private AzureFridayClient _client;
        private ILogger<AzureFridayDB> _logger;
        private IAppCache _cache;

        public AzureFridayDB(AzureFridayClient client, IAppCache appCache,
                ILogger<AzureFridayDB> logger)
        {
            _client = client;
            _logger = logger;
            _cache = appCache;
        }
        public async Task<List<Episode>> GetVideos()
        {
            Func<Task<List<Episode>>> videoObjectFactory = () => PopulateVideosCache();
            var retVal = await _cache.GetOrAddAsync("videos", videoObjectFactory, DateTimeOffset.Now.AddHours(1));
            return retVal;
        }

        public bool PurgeCache()
        {
            _cache.Remove("videos");
            return true;
        }

        public async Task<List<Episode>> PopulateVideosCache()
        {
            List<Episode> response = await _client.GetVideos();
            return response;
        }
    }

    public class Episode
    {
        public string title { get; init; }
        public string url { get; init; }
        public string description { get; init; }
        public string descriptionAsHtml { get; init; }
        public string entryId { get; init; }
        public DateTime uploadDate { get; init; }
        public string youTubeUrl { get; set; }
        public string thumbnailUrl { get; set; }
    }
}
