using CSS.Models;
using CSS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CSS.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ProfileController(UserManager<ApplicationUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        // ==========================
        // SHOW PROFILE
        // ==========================
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            return View(user);
        }

        // ==========================
        // EDIT – GET
        // ==========================
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

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

                // Education School
                SchoolName = user.SchoolName,
                SchoolGroup = user.SchoolGroup,
                SchoolPassingYear = user.SchoolPassingYear,

                // College
                CollegeName = user.CollegeName,
                CollegeGroup = user.CollegeGroup,
                CollegePassingYear = user.CollegePassingYear,

                // University
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
                ProfileImagePath = user.ProfileImagePath
            };

            return View(vm);
        }

        // ==========================
        // EDIT – POST
        // ==========================
        [HttpPost]
        public async Task<IActionResult> Edit(ProfileVM model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null) return RedirectToAction("Login", "Account");

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

            //date count
            // AUTO CALCULATE Donation Eligibility
            if (model.LastDonationDate != null)
            {
                var last = model.LastDonationDate.Value;
                var nextAllowed = last.AddMonths(3);

                model.NextAvailableDonationDate = nextAllowed;

                if (DateTime.Today >= nextAllowed)
                    model.DonationEligibility = "Eligible";
                else
                    model.DonationEligibility = "Ineligible";
            }
            else
            {
                model.DonationEligibility = "Unknown";
            }


            // ==========================
            // IMAGE UPLOAD
            // ==========================
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
    }
}
