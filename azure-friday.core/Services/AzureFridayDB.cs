using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace azure_friday.core.services
{
    public class AzureFridayDB : IAzureFridayDB
    {
        public List<Video> GetVideos()
        {
            string videoListJson = File.ReadAllText(@"Database\af.json");

            List<Video> VideoList = JsonConvert.DeserializeObject<List<Video>>(videoListJson);

            return VideoList;
        }
    }

    public class Video
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public string Host { get; set; }
        public string Guest { get; set; }
        public int Episode { get; set; }
        public string Live { get; set; }
        public string Url { get; set; }
        public string embedUrl { get; set; }
    }
}