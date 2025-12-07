using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class WhatWeDoItemsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public WhatWeDoItemsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ==========================================================
        // LIST PAGE
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            var list = await _db.WhatWeDoItems.ToListAsync();
            return View(list);
        }

        // ==========================================================
        // DETAILS
        // ==========================================================
        public async Task<IActionResult> Details(int id)
        {
            var item = await _db.WhatWeDoItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        // ==========================================================
        // CREATE (GET)
        // ==========================================================
        public IActionResult Create()
        {
            return View(new WhatWeDoItem());
        }

        // ==========================================================
        // CREATE (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WhatWeDoItem model, IFormFile? ImageFile)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (ImageFile == null || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageData", "Image is required.");
                return View(model);
            }

            if (!ImageFile.ContentType.StartsWith("image/"))
            {
                ModelState.AddModelError("ImageData", "Invalid image file.");
                return View(model);
            }

            using (var ms = new MemoryStream())
            {
                await ImageFile.CopyToAsync(ms);
                model.ImageData = ms.ToArray();
            }

            _db.WhatWeDoItems.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // EDIT (GET)
        // ==========================================================
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.WhatWeDoItems.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // ==========================================================
        // EDIT (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WhatWeDoItem model, IFormFile? ImageFile)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var item = await _db.WhatWeDoItems.FindAsync(id);
            if (item == null) return NotFound();

            item.Title = model.Title;
            item.Description = model.Description;
            item.IconClass = model.IconClass;

            if (ImageFile != null && ImageFile.Length > 0)
            {
                if (!ImageFile.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("ImageData", "Invalid image file.");
                    return View(model);
                }

                using (var ms = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(ms);
                    item.ImageData = ms.ToArray();
                }
            }

            _db.WhatWeDoItems.Update(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // DELETE (GET)
        // ==========================================================
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.WhatWeDoItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();

            return View(item);
        }

        // ==========================================================
        // DELETE (POST)
        // ==========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.WhatWeDoItems.FindAsync(id);
            if (item == null) return NotFound();

            _db.WhatWeDoItems.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
