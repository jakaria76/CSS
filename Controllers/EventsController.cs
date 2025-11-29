using System;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using CSS.Data;
using CSS.Models;
using CSS.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EventsController> _logger;

        public EventsController(ApplicationDbContext db, ILogger<EventsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        // =================================================
        // EXPORT REGISTRATIONS
        // =================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportRegistrationsExcel(int id)
        {
            var ev = await _db.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Registrations");

            ws.Cell(1, 1).Value = "Full Name";
            ws.Cell(1, 2).Value = "Mobile";
            ws.Cell(1, 3).Value = "Email";
            ws.Cell(1, 4).Value = "Gender";
            ws.Cell(1, 5).Value = "Institute";
            ws.Cell(1, 6).Value = "Class";
            ws.Cell(1, 7).Value = "Volunteer?";
            ws.Cell(1, 8).Value = "Why Join";
            ws.Cell(1, 9).Value = "Payment Method";
            ws.Cell(1, 10).Value = "Registered At";

            int r = 2;
            foreach (var x in ev.Registrations.OrderBy(x => x.RegisteredAt))
            {
                ws.Cell(r, 1).Value = x.FullName;
                ws.Cell(r, 2).Value = x.Mobile;
                ws.Cell(r, 3).Value = x.Email;
                ws.Cell(r, 4).Value = x.Gender;
                ws.Cell(r, 5).Value = x.InstituteName;
                ws.Cell(r, 6).Value = x.ClassName;
                ws.Cell(r, 7).Value = x.WillVolunteer ? "Yes" : "No";
                ws.Cell(r, 8).Value = x.WhyJoin;
                ws.Cell(r, 9).Value = x.PaymentMethod;
                ws.Cell(r, 10).Value = x.RegisteredAt.ToString("yyyy-MM-dd HH:mm");
                r++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Event_{ev.Id}_Registrations.xlsx");
        }

        // =================================================
        // INDEX
        // =================================================
        public async Task<IActionResult> Index(string? category)
        {
            var now = DateTime.UtcNow;

            var all = await _db.Events
                .Include(e => e.Images)
                .Where(e => e.IsPublished)
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();

            var featured = all.Where(e => e.IsFeatured && (e.StartDateTime ?? DateTime.MaxValue) >= now).ToList();
            var upcoming = all.Where(e => (e.StartDateTime ?? DateTime.MaxValue) >= now).ToList();
            var past = all.Where(e => (e.StartDateTime ?? DateTime.MinValue) < now)
                          .OrderByDescending(e => e.StartDateTime)
                          .ToList();

            var categories = all
                .SelectMany(e => (e.Tag ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct()
                .ToList();

            if (!string.IsNullOrEmpty(category))
            {
                upcoming = upcoming.Where(e => (e.Tag ?? "").Contains(category, StringComparison.OrdinalIgnoreCase)).ToList();
                past = past.Where(e => (e.Tag ?? "").Contains(category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(new EventListVM
            {
                Featured = featured,
                Upcoming = upcoming,
                Past = past,
                Categories = categories,
                SelectedCategory = category
            });
        }

        // =================================================
        // DETAILS
        // =================================================
        public async Task<IActionResult> Details(int id)
        {
            var ev = await _db.Events
                .Include(e => e.Images)
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            return View(new EventDetailsVM
            {
                Event = ev,
                Images = ev.Images,
                RegisteredCount = ev.Registrations?.Count ?? 0
            });
        }

        // =================================================
        // REGISTER
        // =================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int eventId, string FullName, string Mobile, string? Email,
            string? Gender, string? InstituteName, string? ClassName, string? WhyJoin,
            string? PaymentMethod, bool WillVolunteer, IFormFile? UserImage)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound();

            var reg = new EventRegistration
            {
                EventId = eventId,
                FullName = FullName,
                Mobile = Mobile,
                Email = Email,
                Gender = Gender,
                InstituteName = InstituteName,
                ClassName = ClassName,
                WhyJoin = WhyJoin,
                PaymentMethod = PaymentMethod,
                WillVolunteer = WillVolunteer,
                UserId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            };

            if (UserImage != null)
            {
                using var ms = new MemoryStream();
                await UserImage.CopyToAsync(ms);
                reg.UserImage = ms.ToArray();
                reg.UserImageType = UserImage.ContentType;
            }

            _db.EventRegistrations.Add(reg);
            await _db.SaveChangesAsync();

            TempData["Registered"] = "Registration successful!";
            return RedirectToAction("Details", new { id = eventId });
        }

        // =================================================
        // CREATE
        // =================================================
        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View(new EventCreateVM());

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateVM vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var ev = new Event
            {
                Title = vm.Title,
                Tag = vm.Tag,
                ShortDescription = vm.ShortDescription,
                FullDescription = vm.FullDescription,
                StartDateTime = vm.StartDateTime,
                EndDateTime = vm.EndDateTime,
                Venue = vm.Venue,
                Latitude = vm.Latitude,
                Longitude = vm.Longitude,
                OrganizedBy = vm.OrganizedBy,
                ContactPerson = vm.ContactPerson,
                ContactPhone = vm.ContactPhone,
                ExpectedParticipants = vm.ExpectedParticipants,
                VolunteersNeeded = vm.VolunteersNeeded,
                IsPublished = vm.IsPublished || true,
                IsFeatured = vm.IsFeatured
            };

            if (vm.BannerImage != null)
            {
                using var ms = new MemoryStream();
                await vm.BannerImage.CopyToAsync(ms);
                ev.BannerImage = ms.ToArray();
                ev.BannerImageType = vm.BannerImage.ContentType;
            }

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            if (vm.GalleryImages != null)
            {
                foreach (var file in vm.GalleryImages)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);
                    _db.EventImages.Add(new EventImage
                    {
                        EventId = ev.Id,
                        ImageData = ms.ToArray(),
                        ImageType = file.ContentType
                    });
                }
                await _db.SaveChangesAsync();
            }

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction("Index");
        }

        // =================================================
        // EDIT
        // =================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            return View(new EventCreateVM
            {
                Id = ev.Id,
                Title = ev.Title,
                Tag = ev.Tag,
                ShortDescription = ev.ShortDescription,
                FullDescription = ev.FullDescription,
                StartDateTime = ev.StartDateTime,
                EndDateTime = ev.EndDateTime,
                Venue = ev.Venue,
                Latitude = ev.Latitude,
                Longitude = ev.Longitude,
                OrganizedBy = ev.OrganizedBy,
                ContactPerson = ev.ContactPerson,
                ContactPhone = ev.ContactPhone,
                ExpectedParticipants = ev.ExpectedParticipants,
                VolunteersNeeded = ev.VolunteersNeeded,
                IsPublished = ev.IsPublished || true,
                IsFeatured = ev.IsFeatured
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(EventCreateVM vm)
        {
            var ev = await _db.Events.Include(e => e.Images)
                .FirstOrDefaultAsync(e => e.Id == vm.Id);

            if (ev == null) return NotFound();

            ev.Title = vm.Title;
            ev.Tag = vm.Tag;
            ev.ShortDescription = vm.ShortDescription;
            ev.FullDescription = vm.FullDescription;
            ev.StartDateTime = vm.StartDateTime;
            ev.EndDateTime = vm.EndDateTime;
            ev.Venue = vm.Venue;
            ev.Latitude = vm.Latitude;
            ev.Longitude = vm.Longitude;
            ev.OrganizedBy = vm.OrganizedBy;
            ev.ContactPerson = vm.ContactPerson;
            ev.ContactPhone = vm.ContactPhone;
            ev.ExpectedParticipants = vm.ExpectedParticipants;
            ev.VolunteersNeeded = vm.VolunteersNeeded;
            ev.IsPublished = vm.IsPublished || true;
            ev.IsFeatured = vm.IsFeatured;

            if (vm.BannerImage != null)
            {
                using var ms = new MemoryStream();
                await vm.BannerImage.CopyToAsync(ms);
                ev.BannerImage = ms.ToArray();
                ev.BannerImageType = vm.BannerImage.ContentType;
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Event updated successfully!";
            return RedirectToAction("Index");
        }

        // =================================================
        // DELETE
        // =================================================
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _db.Events
                .Include(e => e.Images)
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null) return NotFound();

            _db.EventImages.RemoveRange(ev.Images);
            _db.EventRegistrations.RemoveRange(ev.Registrations);
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction("Index");
        }
    }
}
