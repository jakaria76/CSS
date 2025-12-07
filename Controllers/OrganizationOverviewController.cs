using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OrganizationOverviewController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrganizationOverviewController(ApplicationDbContext db)
        {
            _db = db;
        }

        // INDEX
        public async Task<IActionResult> Index()
        {
            var overview = await _db.OrganizationOverviews.FirstOrDefaultAsync();
            return View(overview);
        }

        // CREATE (GET)
        public IActionResult Create()
        {
            return View(new OrganizationOverview());
        }

        // CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrganizationOverview model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.OrganizationOverviews.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var overview = await _db.OrganizationOverviews.FindAsync(id);
            if (overview == null) return NotFound();

            return View(overview);
        }

        // EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrganizationOverview model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var overview = await _db.OrganizationOverviews.FindAsync(id);
            if (overview == null) return NotFound();

            overview.Description = model.Description;
            overview.FoundedYear = model.FoundedYear;
            overview.Purpose = model.Purpose;

            _db.OrganizationOverviews.Update(overview);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
