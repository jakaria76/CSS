using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class NoticeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NoticeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var notices = await _db.Notices
                .OrderByDescending(n => n.PublishDate)   // Always latest first
                .ToListAsync();

            return View(notices);
        }

        // Display PDF inside browser
        public async Task<IActionResult> ViewPdf(int id)
        {
            var notice = await _db.Notices.FindAsync(id);
            if (notice == null)
                return NotFound();

            // Browser shows PDF instead of auto-download
            Response.Headers.Append("Content-Disposition", $"inline; filename={notice.FileName}");

            return File(notice.FileData, "application/pdf");
        }
    }
}
