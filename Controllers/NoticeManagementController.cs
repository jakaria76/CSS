using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class NoticeManagementController : Controller
    {
        private readonly ApplicationDbContext _db;

        public NoticeManagementController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ===== LIST ALL NOTICES (Latest First) =====
        public async Task<IActionResult> Index()
        {
            var notices = await _db.Notices
                .OrderByDescending(n => n.PublishDate)
                .ToListAsync();

            return View(notices);
        }

        // ===== CREATE PAGE =====
        public IActionResult Create()
        {
            return View(new Notice
            {
                PublishDate = DateTime.Now  // Default date + time
            });
        }

        // ===== CREATE ACTION =====
        [HttpPost]
        public async Task<IActionResult> Create(Notice model, IFormFile file)
        {
            ModelState.Remove("FileData");
            ModelState.Remove("FileName");

            if (!ModelState.IsValid)
                return View(model);

            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "PDF file is required.");
                return View(model);
            }

            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);

            model.FileName = file.FileName;
            model.FileData = ms.ToArray();
            model.CreatedAt = DateTime.Now;
            model.CreatedBy = User?.Identity?.Name ?? "admin";

            // IMPORTANT FIX → PublishDate = Selected date + current time
            model.PublishDate = new DateTime(
                model.PublishDate.Year,
                model.PublishDate.Month,
                model.PublishDate.Day,
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second
            );

            _db.Notices.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // ===== EDIT PAGE =====
        public async Task<IActionResult> Edit(int id)
        {
            var notice = await _db.Notices.FindAsync(id);
            if (notice == null)
                return NotFound();

            return View(notice);
        }

        // ===== EDIT ACTION =====
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Notice model, IFormFile? file)
        {
            var notice = await _db.Notices.FindAsync(id);
            if (notice == null)
                return NotFound();

            ModelState.Remove("FileData");
            ModelState.Remove("FileName");

            if (!ModelState.IsValid)
                return View(model);

            notice.Title = model.Title;

            // KEEP DATE & TIME CORRECT
            notice.PublishDate = new DateTime(
                model.PublishDate.Year,
                model.PublishDate.Month,
                model.PublishDate.Day,
                notice.PublishDate.Hour,
                notice.PublishDate.Minute,
                notice.PublishDate.Second
            );

            if (file != null)
            {
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);

                notice.FileName = file.FileName;
                notice.FileData = ms.ToArray();
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // ===== DELETE =====
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var notice = await _db.Notices.FindAsync(id);
            if (notice == null)
                return NotFound();

            _db.Notices.Remove(notice);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
