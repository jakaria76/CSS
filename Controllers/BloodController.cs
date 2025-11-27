using CSS.Data;
using CSS.Models;
using CSS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class BloodController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BloodController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================================
        // INDEX — Show summary of all blood groups
        // ================================================
        public async Task<IActionResult> Index()
        {
            var bloodGroups = new[] { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

            var users = await _context.Users.AsNoTracking().ToListAsync();

            var model = bloodGroups.Select(bg => new BloodGroupVM
            {
                BloodGroup = bg,
                Total = users.Count(u => u.BloodGroup == bg),
                Available = users.Count(u => u.BloodGroup == bg && u.DonationEligibility == "Eligible"),
                NotAvailable = users.Count(u => u.BloodGroup == bg && u.DonationEligibility != "Eligible")
            }).ToList();

            return View(model);
        }

        // ================================================
        // FIND — GET (User selects blood group + picks map)
        // ================================================
        [HttpGet]
        public IActionResult Find(string group)
        {
            ViewBag.Group = group;
            return View();
        }

        // ================================================
        // FIND — POST (Find ALL donors sorted by nearest)
        // ================================================
        [HttpPost]
        public IActionResult Find(string group, double lat, double lng)
        {
            var donors = _context.Users
                        .Where(x => x.BloodGroup == group &&
                                    x.Latitude != null &&
                                    x.Longitude != null &&
                                    x.DonationEligibility == "Eligible")
                        .ToList();

            var nearest = donors
                .Select(d => new DonorWithDistance
                {
                    User = d,
                    DistanceKm = Haversine(lat, lng, d.Latitude!.Value, d.Longitude!.Value)
                })
                .OrderBy(d => d.DistanceKm) // nearest first
                .ToList(); // IMPORTANT: all donors (no limit)

            ViewBag.UserLatitude = lat;
            ViewBag.UserLongitude = lng;
            ViewBag.Group = group;

            return View("Nearest", nearest);
        }

        // ================================================
        // OPTIONAL — AJAX: Return donors JSON (for map)
        // ================================================
        [HttpPost]
        public async Task<IActionResult> GetNearestJson(string group, double lat, double lng, int take = 3)
        {
            var donors = await _context.Users
                .Where(x => x.BloodGroup == group &&
                            x.Latitude != null &&
                            x.Longitude != null)
                .AsNoTracking()
                .ToListAsync();

            var nearest = donors
                .Select(d => new DonorWithDistance
                {
                    User = d,
                    DistanceKm = Haversine(lat, lng, d.Latitude!.Value, d.Longitude!.Value)
                })
                .OrderBy(x => x.DistanceKm)
                .Take(take)
                .ToList();

            var result = nearest.Select(n => new
            {
                Id = n.User.Id,
                Name = n.User.FullName,
                BloodGroup = n.User.BloodGroup,
                DistanceKm = Math.Round(n.DistanceKm, 2),
                Lat = n.User.Latitude,
                Lng = n.User.Longitude,
                Dms = n.User.LocationDms,
                ProfileImage = n.User.ProfileImagePath
            });

            return Json(result);
        }

        // ================================================
        // HAVERSINE — Calculate distance between 2 points
        // ================================================
        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // km

            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRad(double deg) => deg * (Math.PI / 180.0);
    }
}
