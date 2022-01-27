namespace azure_friday.core.services
{
    public interface IAzureFridayDB
    {
        Task<List<Episode>> GetVideos();

        Task<List<Episode>> PopulateVideosCache();

        public bool PurgeCache();
    }
}