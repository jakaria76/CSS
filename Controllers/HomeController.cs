using CSS.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using CSS.Models;

namespace CSS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 5;

            // ===================== NOTICE PAGINATION =====================
            var totalNotices = await _db.Notices.CountAsync();
            var totalPages = (int)Math.Ceiling(totalNotices / (double)pageSize);

            var notices = await _db.Notices
                .OrderByDescending(n => n.PublishDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();


            // ===================== UPCOMING EVENTS =====================
            var upcomingEvents = await _db.Events
                .Where(e => e.StartDateTime != null
                         && e.StartDateTime > DateTime.Now
                         && e.IsPublished == true)
                .OrderBy(e => e.StartDateTime)
                .Take(4)
                .ToListAsync();

            ViewBag.UpcomingEvents = upcomingEvents;


            // ===================== LATEST / SORTED VIDEOS =====================
            var videos = await _db.Videos
                .OrderBy(v => v.SortOrder)            // Admin-defined sorting
                .ThenByDescending(v => v.CreatedAt)   // Latest first
                .Take(6)
                .ToListAsync();                       // IMPORTANT: No Select()


            ViewBag.VideoList = videos;  // Now this contains List<Video>


            // ===================== NOTICE PAGINATION VIEWBAG =====================
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;


            // RETURN VIEW
            return View(notices);
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel());
        }
    }
}
