using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ContactInfosController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ContactInfosController(ApplicationDbContext db)
        {
            _db = db;
        }

        // ==========================================================
        // INDEX — Show contact info or Create button
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            var info = await _db.ContactInfos.FirstOrDefaultAsync();
            return View(info);
        }

        // ==========================================================
        // CREATE (GET)
        // ==========================================================
        public IActionResult Create()
        {
            return View(new ContactInfo());
        }

        // ==========================================================
        // CREATE (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactInfo model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.ContactInfos.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // EDIT (GET)
        // ==========================================================
        public async Task<IActionResult> Edit(int id)
        {
            var info = await _db.ContactInfos.FindAsync(id);
            if (info == null) return NotFound();

            return View(info);
        }

        // ==========================================================
        // EDIT (POST)
        // ==========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactInfo model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var info = await _db.ContactInfos.FindAsync(id);
            if (info == null) return NotFound();

            info.Email = model.Email;
            info.Phone = model.Phone;
            info.Address = model.Address;
            info.FacebookUrl = model.FacebookUrl;
            info.WebsiteUrl = model.WebsiteUrl;

            _db.ContactInfos.Update(info);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
