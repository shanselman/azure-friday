using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using azure_friday.core.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace azure_friday.core.Pages
{
    public class IndexModel : PageModel
    {
        private IAzureFridayDB _db;
        public IEnumerable<Video> Videos { get; set; }
        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; }
        public string CurrentSearch { get; set; }
        public bool ShowPrevious { get; set; }
        public bool ShowNext { get; set; }
        public bool ShowFirst { get; set; }
        public bool ShowLast { get; set; }

        public IndexModel(IAzureFridayDB db)
        {
            _db = db;
        }
        public IActionResult OnGet(int currentpage, string search, string minDate, string maxDate)
        {
            var videos = _db.GetVideos();
            MinDate = videos.Min(x => DateTime.Parse(x.Live));
            MaxDate = videos.Max(x => DateTime.Parse(x.Live));

            // track current page user is on
            CurrentPage = currentpage == 0 ? 1 : currentpage;

            if (!string.IsNullOrEmpty(search))
            {
                CurrentSearch = search;
                // create a collection of words from search param to be used in query
                string[] queryWords = search.Split(' ');

                // check titles that contain search params
                // Videos = videos.Where(v => v.Title.ToLower().Contains(search.ToLower()))

                // check titles that contain at least one or more words in search param
                var query = videos.Where(x => queryWords.Any(q => x.Title.ToLower().Contains(q.ToLower())));
                Videos = query
                .Skip(((currentpage - 1) * 16))
                .Take(16);
                // get number of pages, divide total video count by 16 (which is the number of videos shown per page) then check if there is a remainder
                // add 1 to, using this for pagination
                PageCount = (query.Count() / 16) + (query.Count() % 16 > 0 ? 1 : 0);
            }
            else
            {
                Videos = videos.Skip(((currentpage - 1) * 16)).Take(16);
                // get number of pages, divide total video count by 16 (which is the number of videos shown per page) then check if there is a remainder
                // add 1 to, using this for pagination
                PageCount = (videos.Count / 16) + (videos.Count % 16 > 0 ? 1 : 0);
            }

            
            ShowPrevious = CurrentPage > 1;
            ShowNext = CurrentPage < PageCount;
            ShowFirst = CurrentPage != 1;
            ShowLast = CurrentPage < PageCount;

            return Page();
        }
    }
}
