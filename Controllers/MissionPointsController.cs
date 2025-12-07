using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MissionPointsController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MissionPointsController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ==========================================================
        // LIST PAGE
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            var list = await _db.MissionPoints.ToListAsync();
            return View(list);
        }

        // ==========================================================
        // CREATE (GET)
        // ==========================================================
        public IActionResult Create()
        {
            return View(new MissionPoint());
        }

        // ==========================================================
        // CREATE (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MissionPoint model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.MissionPoints.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // EDIT (GET)
        // ==========================================================
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _db.MissionPoints.FindAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }

        // ==========================================================
        // EDIT (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MissionPoint model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var item = await _db.MissionPoints.FindAsync(id);
            if (item == null) return NotFound();

            item.Text = model.Text;

            _db.MissionPoints.Update(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // DELETE (GET)
        // ==========================================================
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.MissionPoints.FirstOrDefaultAsync(x => x.Id == id);
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
            var item = await _db.MissionPoints.FindAsync(id);
            if (item == null) return NotFound();

            _db.MissionPoints.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
