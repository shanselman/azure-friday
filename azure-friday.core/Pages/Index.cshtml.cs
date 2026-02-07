using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using azure_friday.core.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Net.Http.Headers;

namespace azure_friday.core.Pages {
    public class IndexModel : PageModel {
        private IAzureFridayDB _db;
        public List<Episode> Episodes { get; set; }

        public IndexModel(IAzureFridayDB db) {
            _db = db;
        }

        public async Task<IActionResult> OnGet(int? id, string path) {

            //did the url have an id?
            // left pad with zeros 12 => 012 
            // Redirect to https://aka.ms/azfr/{paddedId}
            //else, continue 
            if (id.HasValue)
            {
                return Redirect($"https://aka.ms/azfr/{id:000}");
            }

            // Get latest episode thumbnail for og:image (uses cached data)
            var videos = await _db.GetVideos();
            var latestThumbnail = videos?.FirstOrDefault()?.thumbnailUrl;
            if (!string.IsNullOrEmpty(latestThumbnail))
            {
                ViewData["OgImage"] = latestThumbnail;
            }

            return Page();        
        }

        public async Task<JsonResult> OnGetLoadVideos () {
            var videos = await _db.GetVideos ();
            this.HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + (60*60*4); //four hours
            return new JsonResult (videos);
        }

        /// <summary>
        /// Purges the in-memory video cache (4-hour LazyCache).
        /// Call via POST to /?handler=PurgeCache with a valid antiforgery token.
        /// This forces the next request to re-fetch episodes from the external API.
        /// Useful after a new episode is published and you don't want to wait for 
        /// the cache to expire naturally. Only accessible via POST with antiforgery 
        /// token validation (default Razor Pages behavior), preventing CSRF attacks.
        /// </summary>
        public Microsoft.AspNetCore.Mvc.ActionResult OnPostPurgeCache()
        {
            _db.PurgeCache();
            return new OkResult();
        }
    }
}