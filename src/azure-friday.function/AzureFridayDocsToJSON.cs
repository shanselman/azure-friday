using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AFAF
{
    public class AzureFridayDocsToJSON
    {
        private readonly ILogger _logger;

        public AzureFridayDocsToJSON(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AzureFridayDocsToJSON>();
        }

        [Function("AzureFridayDocsToJSON")]
        public async Task<Outputs> Run([TimerTrigger("5 10 * * *", RunOnStartup = true)] MyInfo myTimer)
        {

            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");

            List<Episode> episodes = await DocsToDump.GetEpisodeList();

            var output = new Outputs();

            var stream = new MemoryStream();
            await AFAF.DocsToDump.DumpDoc(stream, episodes, AFAF.Format.Json);
            stream.Position = 0;
            output.AzureFridayApi = System.Text.Encoding.ASCII.GetString(stream.ToArray());


            stream = new MemoryStream();
            await AFAF.DocsToDump.DumpDoc(stream, episodes, AFAF.Format.Rss);
            stream.Position = 0;
            output.AzureFridayRss = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            stream = new MemoryStream();
            await AFAF.DocsToDump.DumpDoc(stream, episodes, AFAF.Format.RssAudio);
            stream.Position = 0;
            output.AzureFridayAudioRss = System.Text.Encoding.ASCII.GetString(stream.ToArray());

            return output;
        }
    }

    public class Outputs
    {
        [BlobOutput("output/azurefriday.json")]
        public string? AzureFridayApi { get; set; }

        [BlobOutput("output/azurefridayaudio.rss")]
        public string? AzureFridayAudioRss { get; set; }

        [BlobOutput("output/azurefriday.rss")]
        public string? AzureFridayRss { get; set; }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
