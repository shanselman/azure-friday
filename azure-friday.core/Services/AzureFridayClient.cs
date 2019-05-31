using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace azure_friday.core.services
{
    public class AzureFridayClient
    {
        private HttpClient _client;
        public IConfiguration Configuration { get; }

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

        public async Task<Response> GetVideos()
        {
            try
            {
                var response = await _client.GetAsync(Configuration["AzureFridayApi"]);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<Response>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to AzureFriday API {ex.ToString()}");
                throw;
            }

        }

    }

}