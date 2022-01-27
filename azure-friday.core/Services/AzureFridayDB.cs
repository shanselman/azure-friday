using LazyCache;

namespace azure_friday.core.services
{
    public class AzureFridayDB : IAzureFridayDB
    {
        private IAppCache _cache;
        private AzureFridayClient _client;
        private ILogger<AzureFridayDB> _logger;

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
            var retVal = await _cache.GetOrAddAsync("videos", videoObjectFactory, DateTimeOffset.Now.AddHours(4));
            return retVal;
        }

        public async Task<List<Episode>> PopulateVideosCache()
        {
            List<Episode> response = await _client.GetVideos();
            return response;
        }

        public bool PurgeCache()
        {
            _cache.Remove("videos");
            return true;
        }
    }

    public class Episode
    {
        public string description { get; init; }
        public string descriptionAsHtml { get; init; }
        public string entryId { get; init; }
        public string thumbnailUrl { get; set; }
        public string title { get; init; }
        public DateTime uploadDate { get; init; }
        public string url { get; init; }
        public string youTubeUrl { get; set; }
    }

    //public static List<Episode> FromJson(string json) => JsonConvert.DeserializeObject<List<Episode>>(json);
}