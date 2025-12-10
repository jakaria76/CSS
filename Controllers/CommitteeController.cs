using CSS.Data;
using CSS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class CommitteeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommitteeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string category, string search)
        {
            // Position groups
            var topLevel = new List<string>
            {
                "সভাপতি","সহ-সভাপতি","সাধারণ সম্পাদক","যুগ্ম-সাধারণ সম্পাদক"
            };

            var executive = new List<string>
            {
                "সাংগঠনিক সম্পাদক","সহ-সাংগঠনিক সম্পাদক",
                "দপ্তর সম্পাদক","সিনিয়র সহ-দপ্তর সম্পাদক","সহ-দপ্তর সম্পাদক",
                "অর্থ সম্পাদক","সিনিয়র অর্থ সম্পাদক","সহ-অর্থ সম্পাদক",
                "শিক্ষা সম্পাদক","সহ-শিক্ষা সম্পাদক",
                "পরিকল্পনা সম্পাদক","সহ-পরিকল্পনা সম্পাদক",
                "মানব সম্পদ সম্পাদক","সহ-মানব সম্পদ সম্পাদক",
                "পরিবেশ সম্পাদক","সহ-পরিবেশ সম্পাদক",
                "ধর্ম সম্পাদক","সহ-ধর্ম সম্পাদক",
                "প্রচার সম্পাদক","সহ-প্রচার সম্পাদক",
                "ব্র্যান্ড ও গণমাধ্যম সম্পাদক","সিনিয়র ব্র্যান্ড ও গণমাধ্যম সম্পাদক",
                "গ্রাফিক্স ডিজাইনার","সহ-গ্রাফিক্স ডিজাইনার",
                "ক্রিয়া সম্পাদক","সহ-ক্রিয়া সম্পাদক",
                "পাঠাগার সম্পাদক","সহ-পাঠাগার সম্পাদক",
                "সাংস্কৃতিক সম্পাদক","সহ-সাংস্কৃতিক সম্পাদক",
                "বিজ্ঞান ও প্রযুক্তি সম্পাদক","সহ-বিজ্ঞান ও প্রযুক্তি সম্পাদক",
                "সমাজ কল্যাণ সম্পাদক","সহ-সমাজ কল্যাণ সম্পাদক",
                "স্বাস্থ্য সম্পাদক","সহ-স্বাস্থ্য সম্পাদক",
                "নারী সম্পাদক","সহ-নারী সম্পাদক",
                "আন্তর্জাতিক সম্পাদক","সহ-আন্তর্জাতিক সম্পাদক",
                "ছাত্র কল্যাণ সম্পাদক","সহ-ছাত্র কল্যাণ সম্পাদক",
                "সাহিত্য সম্পাদক","সহ-সাহিত্য সম্পাদক",
                "তথ্য ও গবেষণা সম্পাদক","সহ-তথ্য ও গবেষণা সম্পাদক",
                "ত্রাণ ও দুর্যোগ সম্পাদক","সিনিয়র ত্রাণ ও দুর্যোগ সম্পাদক",
                "সহ-ত্রাণ ও দুর্যোগ সম্পাদক"
            };

            var generalMembers = new List<string>
            {
                "কার্যকরী সদস্য"
            };

            // Load all committee members
            var members = await _context.Users
                .Where(x => x.MemberType == "Committee")
                .ToListAsync();

            // Category filter
            if (!string.IsNullOrEmpty(category))
            {
                if (category == "Top")
                    members = members.Where(x => topLevel.Contains(x.CommitteePosition)).ToList();
                else if (category == "Executive")
                    members = members.Where(x => executive.Contains(x.CommitteePosition)).ToList();
                else if (category == "Members")
                    members = members.Where(x => generalMembers.Contains(x.CommitteePosition)).ToList();
            }

            // Search filter
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                members = members.Where(x =>
                    x.FullName.ToLower().Contains(search) ||
                    x.CommitteePosition.ToLower().Contains(search)
                ).ToList();
            }

            // Sorting Logic
            var orderedList = topLevel.Concat(executive).Concat(generalMembers).ToList();

            members = members
                .OrderBy(m => orderedList.IndexOf(m.CommitteePosition))
                .ThenBy(m => m.FullName)
                .ToList();

            // Map → ViewModel
            var vm = members.Select(m => new CommitteeMemberVM
            {
                Id = m.Id,
                FullName = m.FullName,
                CommitteePosition = m.CommitteePosition,
                ProfileImagePath = string.IsNullOrEmpty(m.ProfileImagePath)
                    ? "/profiles/default.png"
                    : m.ProfileImagePath
            }).ToList();

            return View(vm);
        }
    }
}
