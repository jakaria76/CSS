using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class AdvisorsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdvisorsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ============================
        //     GET: /Advisors
        // ============================
        public async Task<IActionResult> Index()
        {
            var list = await _db.Advisors.AsNoTracking().ToListAsync();
            return View(list);
        }

        // ============================
        //     GET: /Advisors/Details/5
        // ============================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            var advisor = await _db.Advisors.FirstOrDefaultAsync(a => a.Id == id);
            if (advisor == null) return NotFound();

            return View(advisor);  // Views/Advisors/Details.cshtml
        }

        // ============================
        //     GET: /Advisors/Create
        // ============================
        public IActionResult Create()
        {
            return View(new Advisor());
        }

        // ============================
        //     POST: /Advisors/Create
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Advisor model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            // IMAGE REQUIRED
            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageData", "Image is required.");
                return View(model);
            }

            if (!ImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("ImageData", "The uploaded file must be an image.");
                return View(model);
            }

            using (var ms = new MemoryStream())
            {
                await ImageFile.CopyToAsync(ms);
                model.ImageData = ms.ToArray();
            }

            _db.Advisors.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        //       GET: Edit
        // ============================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return BadRequest();

            var advisor = await _db.Advisors.FindAsync(id.Value);
            if (advisor == null) return NotFound();

            return View(advisor);
        }

        // ============================
        //       POST: Edit
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Advisor model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            var advisor = await _db.Advisors.FindAsync(id);
            if (advisor == null) return NotFound();

            advisor.Name = model.Name;
            advisor.Role = model.Role;
            advisor.Bio = model.Bio;

            // Replace image if new uploaded
            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!ImageFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ImageData", "The uploaded file must be an image.");
                    return View(model);
                }

                using (var ms = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(ms);
                    advisor.ImageData = ms.ToArray();
                }
            }

            _db.Advisors.Update(advisor);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ============================
        //       GET: Delete
        // ============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest();

            var advisor = await _db.Advisors.FirstOrDefaultAsync(a => a.Id == id);
            if (advisor == null) return NotFound();

            return View(advisor);
        }

        // ============================
        //     POST: DeleteConfirmed
        // ============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var advisor = await _db.Advisors.FindAsync(id);
            if (advisor == null) return NotFound();

            _db.Advisors.Remove(advisor);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
