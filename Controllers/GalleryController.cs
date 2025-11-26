using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GalleryController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // INDEX PAGE + FILTER
        public async Task<IActionResult> Index(string? category, DateTime? startDate, DateTime? endDate)
        {
            var query = _context.GalleryImages.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(x => x.Category == category);

            if (startDate.HasValue)
                query = query.Where(x => x.UploadDate.Date >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(x => x.UploadDate.Date <= endDate.Value.Date);

            var images = await query.OrderByDescending(x => x.UploadDate).ToListAsync();

            ViewBag.Categories = await _context.GalleryImages
                .Select(x => x.Category)
                .Distinct()
                .ToListAsync();

            ViewBag.SelectedCategory = category;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            return View(images);
        }

        // UPLOAD PAGE VIEW
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload()
        {
            ViewBag.Categories = await _context.GalleryImages
                .Select(x => x.Category)
                .Distinct()
                .ToListAsync();

            return View();
        }

        // POST: UPLOAD IMAGE
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(string category, DateTime uploadDate, IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                TempData["error"] = "Please select an image.";
                return RedirectToAction("Upload");
            }

            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);

            var galleryItem = new GalleryImage
            {
                Category = category,
                UploadDate = uploadDate,
                ImageData = ms.ToArray(),
                ContentType = image.ContentType,
                UploadedBy = (await _userManager.GetUserAsync(User))?.Email
            };

            _context.GalleryImages.Add(galleryItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // DELETE IMAGE
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var img = await _context.GalleryImages.FindAsync(id);
            if (img == null) return NotFound();

            _context.GalleryImages.Remove(img);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
