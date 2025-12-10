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

            var totalNotices = await _db.Notices.CountAsync();
            var totalPages = (int)Math.Ceiling(totalNotices / (double)pageSize);

            var notices = await _db.Notices
                .OrderByDescending(n => n.PublishDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Upcoming events (Uses StartDateTime and IsPublished)
            var upcomingEvents = await _db.Events
                .Where(e => e.StartDateTime != null
                         && e.StartDateTime > DateTime.Now
                         && e.IsPublished == true)
                .OrderBy(e => e.StartDateTime)
                .Take(3)
                .ToListAsync();

            ViewBag.UpcomingEvents = upcomingEvents;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(notices);
        }

        public IActionResult Privacy() => View();

        public IActionResult Error()
        {
            return View(new ErrorViewModel());
        }
    }
}
