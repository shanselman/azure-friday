using AFAF;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace AzureFridayDocstoJSON
{
    public static class AzureDocsToAFJson
    {
        [Function("AzureDocsToPodcastRSS")]
        public static async Task<AzureFridayOutput> RunJsonAsync(
            [TimerTrigger("5 10 * * *", RunOnStartup = true)] TimerInfo myTimer,
            ILogger log)
        {
            List<Episode> episodes = await DocsToDump.GetEpisodeList();

            var output = new AzureFridayOutput();

            MemoryStream dumpJson = new MemoryStream();
            await AFAF.DocsToDump.DumpDoc(dumpJson, episodes, AFAF.Format.Json);
            dumpJson.Position = 0;
            output.AzureFridayApi = System.Text.Encoding.ASCII.GetString(dumpJson.ToArray());


            //await blobJsonClient.UploadAsync(dumpJson, new BlobHttpHeaders { ContentType = "application/json" });

            MemoryStream dumpRss = new MemoryStream();
            await AFAF.DocsToDump.DumpDoc(dumpRss, episodes, AFAF.Format.Rss);
            dumpRss.Position = 0;
            //await blobRssClient.UploadAsync(dumpRss, new BlobHttpHeaders { ContentType = "application/rss+xml" });
            output.AzureFridayRss = System.Text.Encoding.ASCII.GetString(dumpRss.ToArray());

            MemoryStream dumpRssAudio = new MemoryStream();
            await AFAF.DocsToDump.DumpDoc(dumpRssAudio, episodes, AFAF.Format.RssAudio);
            dumpRssAudio.Position = 0;
            output.AzureFridayAudioRss = System.Text.Encoding.ASCII.GetString(dumpRssAudio.ToArray());

            //await blobRssAudioClient.UploadAsync(dumpRssAudio, new BlobHttpHeaders { ContentType = "application/rss+xml" });

            return output;
        }
    }

    public class AzureFridayOutput
    {
        [BlobOutput("output/azurefriday.json")]
        public string AzureFridayApi { get; set; }

        [BlobOutput("output/azurefridayaudio.rss")]
        public string AzureFridayAudioRss { get; set; }

        [BlobOutput("output/azurefriday.rss")]
        public string AzureFridayRss { get; set; }
    }
}
