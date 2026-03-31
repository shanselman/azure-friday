using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using azure_friday.core.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace azure_friday.core.Pages {
    public class IndexModel : PageModel {
        private IAzureFridayDB _db;
        private IConfiguration _configuration;
        public List<Episode> Episodes { get; set; }

        public IndexModel(IAzureFridayDB db, IConfiguration configuration) {
            _db = db;
            _configuration = configuration;
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
            this.HttpContext.Response.Headers[HeaderNames.CacheControl] = "public, max-age=3600, must-revalidate"; //one hour, must revalidate
            return new JsonResult (videos);
        }

        /// <summary>
        /// Purges the in-memory video cache (1-hour LazyCache).
        /// Requires a valid PurgeCacheKey via POST form body with antiforgery token.
        /// </summary>
        [ValidateAntiForgeryToken]
        public Microsoft.AspNetCore.Mvc.ActionResult OnPostPurgeCache([FromForm] string key)
        {
            var expectedKey = _configuration["PurgeCacheKey"];
            if (string.IsNullOrEmpty(expectedKey) || string.IsNullOrEmpty(key))
            {
                return new UnauthorizedResult();
            }

            var keyBytes = System.Text.Encoding.UTF8.GetBytes(key);
            var expectedBytes = System.Text.Encoding.UTF8.GetBytes(expectedKey);
            if (keyBytes.Length != expectedBytes.Length ||
                !CryptographicOperations.FixedTimeEquals(keyBytes, expectedBytes))
            {
                return new UnauthorizedResult();
            }

            _db.PurgeCache();
            return new OkResult();
        }
    }
}