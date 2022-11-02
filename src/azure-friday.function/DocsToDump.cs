 using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
//using Microsoft.Identity.Client;

namespace AFAF
{
    public class DocsToDump
    {
        private const string googlePlayNS = "http://www.google.com/schemas/play-podcasts/1.0";
        private const string iTunesNS = "http://www.itunes.com/dtds/podcast-1.0.dtd";

        //TODO Make this config?
        private const string Description = "Join Scott Hanselman every Friday as he engages one-on-one with the engineers who build the services that power Microsoft Azure as they demo capabilities, answer Scott's questions, and share their insights. Follow us at: friday.azure.com.";
        private const string coauthorName1 = "Scott Hanselman";
        private const string coauthorEmail1 = "scottha@microsoft.com";
        private const string Subtitle = "Real tech conversations with the engineers that built the Azure Cloud.";
        private const string coauthorName2 = "Rob Caron";
        private const string coauthorEmail2 = "robcaron@microsoft.com";
        const string urlMain = "/api/hierarchy/shows/azure-friday/episodes?page={0}&pageSize=30&orderBy=uploaddate%20desc";
        const string urlBatch = "/api/video/public/v1/entries/batch?ids={0}";
        private const string showName = "Azure Friday";
        private const string azureDocsShowUrl = "https://docs.microsoft.com/en-us/shows/azure-friday/";

        public static async Task DumpDoc(Stream outputStream, List<Episode> epList, Format format)
        {
            switch(format)
            {
                case Format.Json:
                    JsonSerializer.Serialize(outputStream, epList, new JsonSerializerOptions() { WriteIndented = true });
                    break;
                case Format.Rss:
                    SerializeToRss(outputStream, epList, false);
                    break;
                case Format.RssAudio:
                    SerializeToRss(outputStream, epList, true);
                    break;
            }
        }

        public static async Task<List<Episode>> GetEpisodeList()
        {
            HttpClient client = new HttpClient();
            // Our "base" URL is Production
            client.BaseAddress = new Uri("https://docs.microsoft.com");
            int pageNumber = 0;
            int totalCount = 0;

            Dictionary<string, Episode> episodes = new Dictionary<string, Episode>();

            while (true)
            {
                string epUrl = String.Format(urlMain, pageNumber);

                string jsonString = await client.GetStringAsync(epUrl);

                Console.WriteLine($"Fetching {epUrl}");
                var jsonObject = JsonNode.Parse(jsonString);
                totalCount = (int)jsonObject["totalCount"].AsValue(); //don't need to do this twice

                JsonNode epNode = jsonObject["episodes"];
                if (epNode?.AsArray() != null && epNode.AsArray().Count == 0) break;

                StringBuilder batchEps = new StringBuilder();

                foreach (JsonObject item in epNode.AsArray())
                {
                    var ep = JsonSerializer.Deserialize<Episode>(item);

                    batchEps.Append(ep.entryId + ",");
                    episodes.Add(ep.entryId, ep);
                }

                string epDetailsUrl = String.Format(urlBatch, batchEps.ToString().TrimEnd(','));
                string jsonDetailsString = await client.GetStringAsync(epDetailsUrl);
                Console.WriteLine($"Fetching {epDetailsUrl}");

                var jsonDetailsObject = JsonNode.Parse(jsonDetailsString);
                foreach (JsonObject item in jsonDetailsObject.AsArray())
                {
                    var entry = item["entry"];
                    var publicVideo = entry["publicVideo"];
                    var thumbnailObj = publicVideo["thumbnailOtherSizes"];
                    var audioUrl = publicVideo["audioUrl"];
                    var lowQualityVideoUrl = publicVideo["lowQualityVideoUrl"];
                    var mediumQualityVideoUrl = publicVideo["mediumQualityVideoUrl"];
                    var highQualityVideoUrl = publicVideo["highQualityVideoUrl"];

                    var captions = publicVideo["captions"].AsArray();


                    var littleThumbnail = thumbnailObj["w800Url"].ToString();

                    string entryId = entry["id"].ToString();
                    string youTubeUrl = entry["youTubeUrl"].ToString();
                    episodes[entryId].thumbnailUrl = UrlFixUp(littleThumbnail).ToString();
                    episodes[entryId].youTubeUrl = youTubeUrl;

                    episodes[entryId].audioUrl = UrlFixUp(audioUrl?.ToString()).ToString();
                    episodes[entryId].lowQualityVideoUrl = UrlFixUp(lowQualityVideoUrl?.ToString()).ToString();
                    episodes[entryId].mediumQualityVideoUrl = UrlFixUp(mediumQualityVideoUrl?.ToString()).ToString();
                    episodes[entryId].highQualityVideoUrl = UrlFixUp(highQualityVideoUrl?.ToString()).ToString();

                    var englishCaptionUrl = captions.FirstOrDefault(e => e["language"]?.ToString() == "en-us")?["url"]?.ToString();
                    var chineseCaptionUrl = captions.FirstOrDefault(e => e["language"]?.ToString() == "zh-cn")?["url"]?.ToString();

                    episodes[entryId].captionsUrlEnUs = UrlFixUp(englishCaptionUrl).ToString();
                    episodes[entryId].captionsUrlZhCn = UrlFixUp(chineseCaptionUrl).ToString();
                }
                pageNumber++;
            }

            List<Episode> epList = episodes.Values.OrderByDescending(e => e.uploadDate).ToList<Episode>();
            return epList;
        }

        private static void SerializeToRss(Stream outputStream, List<Episode> epList, bool audioOnly = false)
        {
            
            XNamespace xiTunesNS = iTunesNS;

            var feed = new SyndicationFeed(
                (audioOnly == true) ? showName + " (Audio)" : showName,
                Description,
                new Uri("https://docs.microsoft.com/en-us/shows/azure-friday/"),
                "FeedID",
                DateTime.Now);

            var xqiTunes = new XmlQualifiedName("itunes", "http://www.w3.org/2000/xmlns/");
            feed.AttributeExtensions.Add(xqiTunes, iTunesNS);

            var xqGooglePlay = new XmlQualifiedName("googleplay", "http://www.w3.org/2000/xmlns/");
            feed.AttributeExtensions.Add(xqGooglePlay, googlePlayNS);

            var scott = new SyndicationPerson(coauthorEmail1, coauthorName1, "https://hanselman.com");
            var rob = new SyndicationPerson(coauthorEmail2, coauthorName2, "https://www.azurefriday.com");
            feed.Authors.Add(scott);
            feed.Authors.Add(rob);
            feed.Contributors.Add(scott);
            feed.Contributors.Add(rob);

            feed.Copyright = new TextSyndicationContent("Copyright " + DateTime.Now.Year);

            feed.Generator = "Custom Azure Functions";
            feed.Id = "FeedID";
            feed.ImageUrl = new Uri("https://hanselstorage.blob.core.windows.net/output/AzureFridaySquareiTunes.png");

            feed.ElementExtensions.Add(new SyndicationElementExtension("author", googlePlayNS, coauthorName1));
            feed.ElementExtensions.Add(new SyndicationElementExtension("email", googlePlayNS, coauthorEmail1));

            feed.ElementExtensions.Add(new SyndicationElementExtension("author", iTunesNS, coauthorName1));

            feed.ElementExtensions.Add(new XElement(xiTunesNS + "image",
                  new XAttribute("href", "https://hanselstorage.blob.core.windows.net/output/AzureFridaySquareiTunes.png")
                        ).CreateReader());

            feed.ElementExtensions.Add(new SyndicationElementExtension("subtitle", iTunesNS, Subtitle));

            feed.ElementExtensions.Add(new XDocument(
                new XElement(xiTunesNS + "owner", new XAttribute(XNamespace.Xmlns + "itunes", xiTunesNS.NamespaceName),
                    new XElement(xiTunesNS + "name", new XAttribute(XNamespace.Xmlns + "itunes", xiTunesNS.NamespaceName), coauthorName1),
                    new XElement(xiTunesNS + "email", new XAttribute(XNamespace.Xmlns + "itunes", xiTunesNS.NamespaceName), coauthorEmail1))).CreateReader());

            feed.ElementExtensions.Add(new SyndicationElementExtension("explicit", iTunesNS, "no"));
            feed.ElementExtensions.Add(new SyndicationElementExtension("summary", iTunesNS, Description));

            feed.ElementExtensions.Add(new SyndicationElementExtension("type", iTunesNS, "episodic"));

            feed.ElementExtensions.Add(new XElement(xiTunesNS + "category",
                  new XAttribute("text", "Technology")
                        ).CreateReader());

            feed.ElementExtensions.Add("pubDate", "", DateTime.UtcNow.ToString("r"));

            //loop starts here
            List<SyndicationItem> items = new List<SyndicationItem>();

            foreach (var ep in epList)
            {
                var textContent = new TextSyndicationContent(ep.descriptionAsHtml);
                var item = new SyndicationItem(
                    ep.title,
                    textContent,
                    new Uri(ep.url),
                    new Uri(ep.url).ToString(),
                    ep.uploadDate);

                item.ElementExtensions.Add(new SyndicationElementExtension("summary", iTunesNS, ep.descriptionAsPlainText));
                item.ElementExtensions.Add(new SyndicationElementExtension("author", iTunesNS, coauthorName1 + ", " + coauthorName2));

                item.ElementExtensions.Add(new SyndicationElementExtension("episodeType", iTunesNS, "full"));

                SyndicationLink enclosureLink = null;
                if (audioOnly)
                {
                    string audioFile = String.Empty;
                    if (!string.IsNullOrEmpty(ep.audioUrl)) audioFile = ep.audioUrl;

                    enclosureLink = SyndicationLink.CreateMediaEnclosureLink(
                        AddPodTracLink(audioFile),
                        "audio/mp3",
                        0);
                }
                else //audio feed
                {
                    string videoFile = String.Empty;
                    if (!string.IsNullOrEmpty(ep.lowQualityVideoUrl)) videoFile = ep.lowQualityVideoUrl;
                    if (!string.IsNullOrEmpty(ep.mediumQualityVideoUrl)) videoFile = ep.mediumQualityVideoUrl;
                    if (!string.IsNullOrEmpty(ep.highQualityVideoUrl)) videoFile = ep.highQualityVideoUrl;

                    enclosureLink = SyndicationLink.CreateMediaEnclosureLink(
                        AddPodTracLink(videoFile),
                        "video/mp4",
                        0);
                }
                item.Links.Add(enclosureLink);

                //manual pubDate because iTunes hates "z"
                item.ElementExtensions.Add(new XElement("pubDate", ep.uploadDate.ToString("r")).CreateReader());
                items.Add(item);
            }

            feed.Items = items;

            feed.Language = "en-us";
            feed.LastUpdatedTime = DateTime.Now;

            SyndicationLink link = new SyndicationLink(new Uri(azureDocsShowUrl), "alternate", showName, "text/html", 7000);
            feed.Links.Add(link);
            SyndicationLink link2 = SyndicationLink.CreateSelfLink(new Uri("https://docs.microsoft.com/en-us/shows/azure-friday/"), "rss/application+xml");
            feed.Links.Add(link2);

            XmlWriter rssWriter = XmlWriter.Create(outputStream, new XmlWriterSettings() { Indent = true });
            Rss20FeedFormatter rssFormatter = new Rss20FeedFormatter(feed);
            rssFormatter.SerializeExtensionsAsAtom = false;

            rssFormatter.WriteTo(rssWriter);
            rssWriter.Close();
        }

        public static Uri? AddPodTracLink(string url)
        {
            Console.WriteLine(url);
            if (String.IsNullOrEmpty(url))
                return null;
            Uri uri = new Uri(UrlFixUp(url));
            Uri retUrl = new Uri("https://dts.podtrac.com/redirect.mp3/" + uri.Host + uri.PathAndQuery + uri.Fragment);
            return retUrl;
        }

        public static string UrlFixUp(string url)
        {
            if (String.IsNullOrEmpty(url))
                return String.Empty;
            url = url.Replace(" ", "%20");
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                return new Uri(url).ToString();
            else
            {
                url = url.Replace("//","/");
                if (url.StartsWith("/video/media"))
                    return new Uri("https://learn.microsoft.com" + url).ToString();
                else
                    return new Uri("https://learn.microsoft.com/video/media" + url).ToString();
            }
        } 
    }

    public enum Format
    {
        Rss, RssAudio, Json
    }

    public record Episode
    {
        public string title { get; init; }

        private string _url;
        public string url { get { return _url; } init { _url = "https://docs.microsoft.com" + value; } }
        public string description { get; init; }

        public string descriptionAsPlainText { get { return Markdig.Markdown.ToPlainText(description); } }
        public string descriptionAsHtml { get { return Markdig.Markdown.ToHtml(description); } }
        public string entryId { get; init; }
        public DateTime uploadDate { get; init; }
        //populated on second pass, via details REST calls
        public string youTubeUrl { get; set; }
        public string thumbnailUrl { get; set; }

        public string audioUrl { get; set; }
        public string lowQualityVideoUrl { get; set; }
        public string mediumQualityVideoUrl { get; set; }
        public string highQualityVideoUrl { get; set; }
        public string captionsUrlEnUs { get; set; }
        public string captionsUrlZhCn { get; set; }
    }

}
