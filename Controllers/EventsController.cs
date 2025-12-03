using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ClosedXML.Excel;
using CSS.Data;
using CSS.Models;
using CSS.ViewModels;
using CSS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSS.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<EventsController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly INotificationService _notificationService;

        public EventsController(
            ApplicationDbContext db,
            ILogger<EventsController> logger,
            IEmailSender emailSender,
            INotificationService notificationService)
        {
            _db = db;
            _logger = logger;
            _emailSender = emailSender;
            _notificationService = notificationService;
        }

        // =====================================================================
        // EXPORT EXCEL
        // =====================================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportRegistrationsExcel(int id)
        {
            var ev = await _db.Events
                .Include(e => e.Registrations)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                return NotFound();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Registrations");

            ws.Cell(1, 1).Value = "Full Name";
            ws.Cell(1, 2).Value = "Mobile";
            ws.Cell(1, 3).Value = "Email";
            ws.Cell(1, 4).Value = "Gender";
            ws.Cell(1, 5).Value = "Institute";
            ws.Cell(1, 6).Value = "Class/Year";
            ws.Cell(1, 7).Value = "Volunteer";
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
                ws.Cell(r, 10).Value = x.RegisteredAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
                r++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);

            return File(ms.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Event_{ev.Id}_Registrations.xlsx");
        }

        // =====================================================================
        // EVENT LIST
        // =====================================================================
        public async Task<IActionResult> Index(string? category)
        {
            var now = DateTime.UtcNow;

            var all = await _db.Events
                .Where(e => e.IsPublished)
                .OrderBy(e => e.StartDateTime)
                .Select(e => new Event
                {
                    Id = e.Id,
                    Title = e.Title,
                    ShortDescription = e.ShortDescription,
                    FullDescription = e.FullDescription,
                    Venue = e.Venue,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    Tag = e.Tag,
                    Price = e.Price,
                    IsFeatured = e.IsFeatured,
                    BannerImage = e.BannerImage,
                    BannerImageType = e.BannerImageType
                })
                .AsNoTracking()
                .ToListAsync();

            var categories = all
                .SelectMany(e => (e.Tag ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(t => t.Trim())
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            if (!string.IsNullOrEmpty(category))
            {
                all = all
                    .Where(e => (e.Tag ?? "")
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Any(t => t.Trim().Equals(category, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(new EventListVM
            {
                Featured = all.Where(e => e.IsFeatured && e.StartDateTime >= now).ToList(),
                Upcoming = all.Where(e => e.StartDateTime >= now).ToList(),
                Past = all.Where(e => e.StartDateTime < now).OrderByDescending(e => e.StartDateTime).ToList(),
                Categories = categories,
                SelectedCategory = category
            });
        }

        // =====================================================================
        // EVENT DETAILS
        // =====================================================================
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

        // =====================================================================
        // FREE EVENT REGISTRATION
        // =====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int eventId, string FullName, string Mobile,
            string? Email, string? Gender, string? InstituteName, string? ClassName,
            string? WhyJoin, string? PaymentMethod, bool WillVolunteer, IFormFile? UserImage)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return NotFound();

            var already = await _db.EventRegistrations
                .AnyAsync(r => r.EventId == eventId &&
                               (r.Mobile == Mobile ||
                               (!string.IsNullOrWhiteSpace(Email) && r.Email == Email)));

            if (already)
            {
                TempData["Error"] = "This mobile number or email is already registered for this event.";
                return RedirectToAction("Details", new { id = eventId });
            }

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
                RegisteredAt = DateTime.UtcNow,
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

            TempData["Registered"] = "Registration completed successfully!";
            return RedirectToAction("Details", new { id = eventId });
        }

        // =====================================================================
        // CREATE EVENT
        // =====================================================================
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
                IsFeatured = vm.IsFeatured,
                Price = vm.Price ?? 0
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

            // ===========================================================
            // SEND EMAIL TO ALL USERS (BACKGROUND MODE → SUPER FAST)
            // ===========================================================
            _ = Task.Run(async () =>
            {
                var emails = await _db.Users
                    .Where(u => u.Email != null)
                    .Select(u => u.Email)
                    .ToListAsync();

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(
                            email,
                            "New Event Published",
                            $"A new event '<b>{ev.Title}</b>' has been created."
                        );
                    }
                    catch { }
                }
            });

            // ===========================================================
            // SEND PUSH NOTIFICATION
            // ===========================================================
            var payload = JsonSerializer.Serialize(new
            {
                notification = new
                {
                    title = "New Event Published",
                    body = ev.Title,
                    icon = "/images/logo512.png",
                    data = new
                    {
                        url = Url.Action("Details", "Events", new { id = ev.Id }, Request.Scheme)
                    }
                }
            });

            await _notificationService.SendToAllUsersAsync(payload);

            TempData["Success"] = "Event created successfully!";
            return RedirectToAction("Index");
        }

        // =====================================================================
        // EDIT EVENT
        // =====================================================================
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
                IsPublished = ev.IsPublished,
                IsFeatured = ev.IsFeatured,
                Price = ev.Price
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(EventCreateVM vm)
        {
            var ev = await _db.Events
                .Include(e => e.Images)
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
            ev.Price = vm.Price ?? 0;

            if (vm.BannerImage != null)
            {
                using var ms = new MemoryStream();
                await vm.BannerImage.CopyToAsync(ms);
                ev.BannerImage = ms.ToArray();
                ev.BannerImageType = vm.BannerImage.ContentType;
            }

            await _db.SaveChangesAsync();

            // BACKGROUND EMAIL
            _ = Task.Run(async () =>
            {
                var emails = await _db.Users
                    .Where(u => u.Email != null)
                    .Select(u => u.Email)
                    .ToListAsync();

                foreach (var email in emails)
                {
                    try
                    {
                        await _emailSender.SendEmailAsync(
                            email,
                            "Event Updated",
                            $"The event '<b>{ev.Title}</b>' has been updated."
                        );
                    }
                    catch { }
                }
            });

            // PUSH
            var payload = JsonSerializer.Serialize(new
            {
                notification = new
                {
                    title = "Event Updated",
                    body = ev.Title,
                    icon = "/images/logo512.png",
                    data = new
                    {
                        url = Url.Action("Details", "Events", new { id = ev.Id }, Request.Scheme)
                    }
                }
            });

            await _notificationService.SendToAllUsersAsync(payload);

            TempData["Success"] = "Event updated successfully!";
            return RedirectToAction("Index");
        }

        // =====================================================================
        // DELETE EVENT
        // =====================================================================
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

            if (ev.Images.Any())
                _db.EventImages.RemoveRange(ev.Images);

            if (ev.Registrations.Any())
                _db.EventRegistrations.RemoveRange(ev.Registrations);

            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event deleted successfully!";
            return RedirectToAction("Index");
        }

        // =====================================================================
        // TEST PUSH
        // =====================================================================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> TestPush()
        {
            var payload = JsonSerializer.Serialize(new
            {
                notification = new
                {
                    title = "🔔 Test Notification",
                    body = "This is a test push notification!",
                    icon = "/images/logo512.png",
                    data = new
                    {
                        url = Url.Action("Index", "Events", null, Request.Scheme)
                    }
                }
            });

            await _notificationService.SendToAllUsersAsync(payload);

            return Content("Test Push Notification Sent!");
        }
    }
}
