namespace azure_friday.core.services
{
    public class AzureFridayClient
    {
        private HttpClient _client;
        private ILogger<AzureFridayClient> _logger;

        public AzureFridayClient(
            HttpClient client,
            IConfiguration configuration,
            ILogger<AzureFridayClient> logger
            )
        {
            Configuration = configuration;
            _logger = logger;
            _client = client;
        }

        public IConfiguration Configuration { get; }

        public async Task<List<Episode>> GetVideos()
        {
            try
            {
                var url = Configuration["AzureFridayApi"];
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<List<Episode>>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to AzureFriday API {ex}");
                throw;
            }
        }
    }
}