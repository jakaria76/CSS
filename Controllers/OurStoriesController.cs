using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OurStoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OurStoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ==========================================================
        // LIST PAGE (Timeline – ordered by EventDate)
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            var list = await _db.OurStories
                                .OrderBy(x => x.EventDate)
                                .ToListAsync();

            return View(list);
        }

        // ==========================================================
        // CREATE (GET)
        // ==========================================================
        public IActionResult Create()
        {
            return View(new OurStory());
        }

        // ==========================================================
        // CREATE (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OurStory model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.OurStories.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // EDIT (GET)
        // ==========================================================
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.OurStories.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // ==========================================================
        // EDIT (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OurStory model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var item = await _db.OurStories.FindAsync(id);
            if (item == null) return NotFound();

            item.EventDate = model.EventDate;
            item.Description = model.Description;

            _db.OurStories.Update(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // DELETE (GET)
        // ==========================================================
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.OurStories.FirstOrDefaultAsync(x => x.Id == id);
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
            var item = await _db.OurStories.FindAsync(id);
            if (item == null) return NotFound();

            _db.OurStories.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
