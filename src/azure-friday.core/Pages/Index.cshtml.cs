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

        public IActionResult OnGet(int? id, string path) {

            //did the url have an id?
            // left pad with zeros 12 => 012 
            // Redirect to https://aka.ms/azfr/{paddedId}
            //else, continue 
            if (id.HasValue)
            {
                return Redirect($"https://aka.ms/azfr/{id:000}");
            }
            return Page();        
        }

        public async Task<JsonResult> OnGetLoadVideos () {
            var videos = await _db.GetVideos ();
            this.HttpContext.Response.Headers[HeaderNames.CacheControl] = "public,max-age=" + (60*60*4); //four hours
            return new JsonResult (videos);
        }

        public Microsoft.AspNetCore.Mvc.ActionResult OnPurgeCache()
        {
            _db.PurgeCache();
            return new OkResult();
        }
    }
}