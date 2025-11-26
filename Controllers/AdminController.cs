using CSS.Models;
using CSS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // =======================
        // LIST + SEARCH
        // =======================
        public async Task<IActionResult> Members(string q)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.ToLower();
                query = query.Where(u =>
                    (u.FullName ?? "").ToLower().Contains(q) ||
                    (u.Email ?? "").ToLower().Contains(q) ||
                    (u.PhoneNumber ?? "").ToLower().Contains(q) ||
                    (u.AlternativeMobile ?? "").ToLower().Contains(q) ||
                    (u.CommitteePosition ?? "").ToLower().Contains(q)
                );
            }

            var users = await query.OrderBy(x => x.FullName).ToListAsync();
            ViewBag.Query = q;

            return View(users);
        }

        // =======================
        // CREATE USER (GET)
        // =======================
        [HttpGet]
        public IActionResult Create()
        {
            return View(new AdminCreateUserVM());
        }

        // =======================
        // CREATE USER (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Create(AdminCreateUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                ModelState.AddModelError("Email", "Email is already registered.");
                return View(model);
            }

            // Auto-generate secure password
            string password = "Aa@" + Guid.NewGuid().ToString("N").Substring(0, 8);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.Mobile,
                AlternativeMobile = model.AlternativeMobile,   // MERGED ✔
                MemberType = model.MemberType,
                CommitteePosition = model.CommitteePosition
            };

            var create = await _userManager.CreateAsync(user, password);

            if (!create.Succeeded)
            {
                foreach (var err in create.Errors)
                    ModelState.AddModelError("", err.Description);
                return View(model);
            }

            return RedirectToAction(nameof(Members));
        }

        // =======================
        // EDIT USER (GET)
        // =======================
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction(nameof(Members));

            var vm = new AdminEditUserVM
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                AlternativeMobile = user.AlternativeMobile,     // MERGED ✔
                MemberType = user.MemberType,
                CommitteePosition = user.CommitteePosition,
                PresentAddress = user.PresentAddress,
                District = user.District
            };

            return View(vm);
        }

        // =======================
        // EDIT USER (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> Edit(AdminEditUserVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return RedirectToAction(nameof(Members));

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.AlternativeMobile = model.AlternativeMobile;    // MERGED ✔
            user.MemberType = model.MemberType;
            user.CommitteePosition = model.CommitteePosition;
            user.PresentAddress = model.PresentAddress;
            user.District = model.District;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Members));
        }

        // =======================
        // EDIT POSITION (GET)
        // =======================
        [HttpGet]
        public async Task<IActionResult> EditPosition(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction(nameof(Members));

            var vm = new AdminCommitteeVM
            {
                Id = user.Id,
                FullName = user.FullName,
                CommitteePosition = user.CommitteePosition
            };

            return View(vm);
        }

        // =======================
        // EDIT POSITION (POST)
        // =======================
        [HttpPost]
        public async Task<IActionResult> EditPosition(AdminCommitteeVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return RedirectToAction(nameof(Members));

            user.CommitteePosition = model.CommitteePosition;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Members));
        }

        // =======================
        // DELETE USER
        // =======================
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Members));
        }
    }
}
