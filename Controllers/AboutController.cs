using CSS.ViewModels;
using CSS.Data;
using CSS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CSS.Controllers
{
    public class AboutController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AboutController(ApplicationDbContext db)
        {
            _db = db;
        }

        // MAIN ABOUT PAGE
        public async Task<IActionResult> Index()
        {
            var vm = new AboutVM
            {
                Advisors = await _db.Advisors.AsNoTracking().ToListAsync(),
                PreviousPresidents = await _db.PreviousPresidents
                                              .OrderByDescending(p => p.TenureEnd)
                                              .ToListAsync(),

                Leadership = await _db.LeadershipMembers.AsNoTracking().ToListAsync(),
                Overview = await _db.OrganizationOverviews.FirstOrDefaultAsync(),
                MissionPoints = await _db.MissionPoints.AsNoTracking().ToListAsync(),
                whatWeDoItems = await _db.WhatWeDoItems.AsNoTracking().ToListAsync(),

                // FIXED: Order by EventDate (NOT by ToString)
                OurStoryList = await _db.OurStories
                                        .OrderBy(s => s.EventDate)
                                        .AsNoTracking()
                                        .ToListAsync(),

                ContactInfo = await _db.ContactInfos.FirstOrDefaultAsync()
            };

            return View(vm);
        }

        // ADVISOR DETAILS
        public async Task<IActionResult> AdvisorDetails(int id)
        {
            var item = await _db.Advisors.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // PREVIOUS PRESIDENT DETAILS
        public async Task<IActionResult> PreviousPresidentDetails(int id)
        {
            var item = await _db.PreviousPresidents.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // LEADERSHIP DETAILS
        public async Task<IActionResult> LeadershipDetails(int id)
        {
            var item = await _db.LeadershipMembers.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }

        // WHAT WE DO DETAILS
        public async Task<IActionResult> WhatWeDoDetails(int id)
        {
            var item = await _db.WhatWeDoItems.FirstOrDefaultAsync(x => x.Id == id);
            if (item == null) return NotFound();
            return View(item);
        }
    }
}
