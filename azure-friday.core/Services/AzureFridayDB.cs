using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


using Newtonsoft.Json.Converters;
using J = Newtonsoft.Json.JsonPropertyAttribute;

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
        public async Task<Response> GetVideos()
        {
            Func<Task<Response>> videoObjectFactory = () => PopulateVideosCache();
            var retVal = await _cache.GetOrAddAsync("videos", videoObjectFactory, DateTimeOffset.Now.AddDays(4));
            return retVal;
        }

        public async Task<Response> PopulateVideosCache()
        {
            Response response = await _client.GetVideos();
            return response;
        }
    }

    public class Video
    {
        private string _largeThumbnail;
        [J("itemLink")]
        public string ItemLink { get; set; }
        [J("itemGroup")]
        public string ItemGroup { get; set; }
        [J("title")]
        public string Title { get; set; }
        [J("published")]
        public string Published { get; set; }
        [J("publishedDate")]
        public string PublishedDate { get; set; }
        [J("body")]
        public string Body { get; set; }
        [J("authors")]
        public string Authors { get; set; }
        [J("containerName")]
        public string ContainerName { get; set; }
        [J("containerLink")]
        public string ContainerLink { get; set; }
        [J("containerBanner")]
        public string ContainerBanner { get; set; }
        [J("containerThumbnail")]
        public string ContainerThumbnail { get; set; }
        [J("largeThumbnail")]
        public string LargeThumbnail
        {
            get
            {
                return _largeThumbnail;
            }
            set
            {
                if (value.Contains("http"))
                {
                    _largeThumbnail = value.Replace("http://video.ch9", "https://sec.ch9");=
                    // _largeThumbnail = value.Replace("http://files.channel9", "https://files.channel9");
                }
                else
                {
                    _largeThumbnail = value.Replace("https://video.ch9", "https://sec.ch9");
                }
            }
        }
        [J("mediumThumbnail")]
        public string MediumThumbnail { get; set; }
        [J("smallThumbnail")]
        public string SmallThumbnail { get; set; }
        [J("videoMP4High")]
        public string VideoMP4High { get; set; }
        [J("videoMP4Medium")]
        public string VideoMP4Medium { get; set; }
        [J("videoMP4Low")]
        public string VideoMP4Low { get; set; }
        [J("videoWMVHQ")]
        public string VideoWMVHQ { get; set; }
        [J("videoWMV")]
        public string VideoWMV { get; set; }
        [J("videoSmooth")]
        public string VideoSmooth { get; set; }
        [J("webTrendsImage")]
        public string WebTrendsImage { get; set; }
    }

    public partial class Response
    {
        [J("items")]
        public List<Video> Items { get; set; }
        [J("totalItems")]
        public int TotalItems { get; set; }
        [J("pageSize")]
        public int PageSize { get; set; }
        [J("PageCount")]
        public int PageCount { get; set; }
        [J("currentPage")]
        public int CurrentPage { get; set; }
    }
    public partial class Response
    {
        public static Response FromJson(string json) => JsonConvert.DeserializeObject<Response>(json);
    }

}