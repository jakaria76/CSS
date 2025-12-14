using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            // ============================================================
            // NOTICE PAGINATION
            // ============================================================
            var totalNotices = await _db.Notices.CountAsync();
            var totalPages = (int)Math.Ceiling(totalNotices / (double)pageSize);

            var notices = await _db.Notices
                .OrderByDescending(n => n.PublishDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // ============================================================
            // UPCOMING EVENTS (HOME PAGE)
            // ============================================================
            var upcomingEvents = await _db.Events
                .Where(e =>
                    e.StartDateTime != null &&
                    e.StartDateTime > DateTime.Now &&
                    e.IsPublished == true)
                .OrderBy(e => e.StartDateTime)
                .Take(4)
                .ToListAsync();

            ViewBag.UpcomingEvents = upcomingEvents;

            // ============================================================
            // VIDEO GALLERY (SORTED)
            // ============================================================
            var videos = await _db.Videos
                .OrderBy(v => v.SortOrder)              // Admin-defined order
                .ThenByDescending(v => v.CreatedAt)     // Latest fallback
                .Take(6)
                .ToListAsync();

            ViewBag.VideoList = videos;

            // ============================================================
            // ADVISORS (RIGHT SIDEBAR)
            // ============================================================
            var advisors = await _db.Advisors
               .OrderBy(a => a.Name)   // ?? Id / CreatedAt
               .Take(3)
               .ToListAsync();

            ViewBag.Advisors = advisors;



            // ============================================================
            // PAGINATION INFO
            // ============================================================
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            // ============================================================
            // RETURN VIEW
            // ============================================================
            return View(notices);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel());
        }
    }
}
