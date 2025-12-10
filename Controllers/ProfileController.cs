using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CSS.Data;
using Microsoft.AspNetCore.Identity;
using CSS.Models;
using CSS.ViewModels;

namespace CSS.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _db = db;
            _userManager = userManager;
            _env = env;
        }

        // ===========================================================
        // SAVE PUSH SUBSCRIPTION
        // ===========================================================
        public class PushSubscriptionModel
        {
            public string endpoint { get; set; } = "";
            public PushKeys keys { get; set; } = new PushKeys();
        }

        public class PushKeys
        {
            public string p256dh { get; set; } = "";
            public string auth { get; set; } = "";
        }

        [HttpPost]
        public async Task<IActionResult> SaveSubscription([FromBody] PushSubscriptionModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.endpoint))
                return BadRequest("Invalid subscription.");

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            user.PushEndpoint = model.endpoint;
            user.PushP256dh = model.keys?.p256dh;
            user.PushAuth = model.keys?.auth;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Subscription saved." });
        }

        // ===========================================================
        // SHOW LOGGED-IN USER PROFILE
        // ===========================================================
        public async Task<IActionResult> Index()
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            if (loggedUser == null)
                return RedirectToAction("Login", "Account");

            return View(loggedUser);
        }

        // ===========================================================
        // PUBLIC PROFILE VIEW  /Profile/View/{id}
        // ===========================================================
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> View(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View("Index", user);
        }

        // ===========================================================
        // EDIT PROFILE — ONLY OWNER CAN ACCESS
        // ===========================================================
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            if (loggedUser == null)
                return RedirectToAction("Login", "Account");

            // Only the logged-in user may edit their own profile
            var user = loggedUser;

            var vm = new ProfileVM
            {
                Id = user.Id,

                // Basic
                FullName = user.FullName,
                FullNameBn = user.FullNameBn,
                MemberType = user.MemberType,
                CommitteePosition = user.CommitteePosition,
                MemberSince = user.MemberSince,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,

                // Contact
                AlternativeMobile = user.AlternativeMobile,
                PresentAddress = user.PresentAddress,
                PermanentAddress = user.PermanentAddress,
                District = user.District,
                Upazila = user.Upazila,
                FacebookLink = user.FacebookLink,
                WhatsAppNumber = user.WhatsAppNumber,

                // Blood
                BloodGroup = user.BloodGroup,
                LastDonationDate = user.LastDonationDate,
                NextAvailableDonationDate = user.NextAvailableDonationDate,
                DonationEligibility = user.DonationEligibility,
                TotalDonationCount = user.TotalDonationCount,
                PreferredDonationLocation = user.PreferredDonationLocation,

                // Education
                SchoolName = user.SchoolName,
                SchoolGroup = user.SchoolGroup,
                SchoolPassingYear = user.SchoolPassingYear,

                CollegeName = user.CollegeName,
                CollegeGroup = user.CollegeGroup,
                CollegePassingYear = user.CollegePassingYear,

                UniversityName = user.UniversityName,
                Department = user.Department,
                StudentId = user.StudentId,
                CurrentYear = user.CurrentYear,
                CurrentSemester = user.CurrentSemester,

                // Bio
                ShortBio = user.ShortBio,
                WhyJoined = user.WhyJoined,
                FutureGoals = user.FutureGoals,
                Hobbies = user.Hobbies,

                // Social
                Facebook = user.Facebook,
                PortfolioWebsite = user.PortfolioWebsite,

                // Image
                ProfileImagePath = user.ProfileImagePath,

                // Location
                Latitude = user.Latitude,
                Longitude = user.Longitude,
                LocationDms = user.LocationDms
            };

            return View(vm);
        }

        // ===========================================================
        // EDIT PROFILE — POST (OWNER ONLY)
        // ===========================================================
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileVM model)
        {
            var loggedUser = await _userManager.GetUserAsync(User);
            if (loggedUser == null)
                return RedirectToAction("Login", "Account");

            // SECURITY: Prevent editing other users
            if (loggedUser.Id != model.Id)
                return Unauthorized();

            var user = loggedUser;

            // BASIC
            user.FullName = model.FullName;
            user.FullNameBn = model.FullNameBn;
            user.MemberType = model.MemberType;
            user.CommitteePosition = model.CommitteePosition;
            user.MemberSince = model.MemberSince;
            user.Gender = model.Gender;
            user.DateOfBirth = model.DateOfBirth;

            // CONTACT
            user.AlternativeMobile = model.AlternativeMobile;
            user.PresentAddress = model.PresentAddress;
            user.PermanentAddress = model.PermanentAddress;
            user.District = model.District;
            user.Upazila = model.Upazila;
            user.FacebookLink = model.FacebookLink;
            user.WhatsAppNumber = model.WhatsAppNumber;

            // BLOOD
            user.BloodGroup = model.BloodGroup;
            user.LastDonationDate = model.LastDonationDate;
            user.NextAvailableDonationDate = model.LastDonationDate?.AddDays(90);
            user.DonationEligibility = model.DonationEligibility;
            user.TotalDonationCount = model.TotalDonationCount;
            user.PreferredDonationLocation = model.PreferredDonationLocation;

            // EDUCATION
            user.SchoolName = model.SchoolName;
            user.SchoolGroup = model.SchoolGroup;
            user.SchoolPassingYear = model.SchoolPassingYear;

            user.CollegeName = model.CollegeName;
            user.CollegeGroup = model.CollegeGroup;
            user.CollegePassingYear = model.CollegePassingYear;

            user.UniversityName = model.UniversityName;
            user.Department = model.Department;
            user.StudentId = model.StudentId;
            user.CurrentYear = model.CurrentYear;
            user.CurrentSemester = model.CurrentSemester;

            // BIO
            user.ShortBio = model.ShortBio;
            user.WhyJoined = model.WhyJoined;
            user.FutureGoals = model.FutureGoals;
            user.Hobbies = model.Hobbies;

            // SOCIAL
            user.Facebook = model.Facebook;
            user.PortfolioWebsite = model.PortfolioWebsite;

            // LOCATION
            user.Latitude = model.Latitude;
            user.Longitude = model.Longitude;

            if (model.Latitude != null && model.Longitude != null)
            {
                user.LocationDms =
                    $"{ConvertToDms(model.Latitude.Value, true)} {ConvertToDms(model.Longitude.Value, false)}";
            }

            // IMAGE UPLOAD
            if (model.ImageFile != null)
            {
                using var ms = new MemoryStream();
                await model.ImageFile.CopyToAsync(ms);
                user.ProfileImageData = ms.ToArray();

                string folder = Path.Combine(_env.WebRootPath, "profiles");
                Directory.CreateDirectory(folder);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                string fullPath = Path.Combine(folder, fileName);

                await System.IO.File.WriteAllBytesAsync(fullPath, user.ProfileImageData);

                user.ProfileImagePath = "/profiles/" + fileName;
            }

            user.LastUpdatedDate = DateTime.UtcNow;
            user.UpdatedBy = user.Id;

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        // ===========================================================
        // HELPERS
        // ===========================================================
        private string ConvertToDms(double coordinate, bool isLatitude)
        {
            var abs = Math.Abs(coordinate);
            var degrees = Math.Floor(abs);
            var minutesRaw = (abs - degrees) * 60;
            var minutes = Math.Floor(minutesRaw);
            var seconds = Math.Round((minutesRaw - minutes) * 60, 2);

            string direction =
                isLatitude ? (coordinate >= 0 ? "N" : "S")
                           : (coordinate >= 0 ? "E" : "W");

            return $"{degrees}°{minutes}'{seconds}\"{direction}";
        }
    }
}
