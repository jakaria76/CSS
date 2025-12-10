using CSS.Data;
using CSS.Helpers;
using CSS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class VideoManagementController : Controller
{
    private readonly ApplicationDbContext _context;

    public VideoManagementController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ========== INDEX ==========
    public async Task<IActionResult> Index()
    {
        var videos = await _context.Videos
            .OrderBy(v => v.SortOrder)
            .ThenByDescending(v => v.CreatedAt)
            .ToListAsync();

        return View(videos);
    }

    // ========== CREATE (GET) ==========
    public IActionResult Create() => View();

    // ========== CREATE (POST) ==========
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Video model)
    {
        if (!ModelState.IsValid) return View(model);

        var id = YouTubeHelper.ExtractYouTubeId(model.YouTubeUrl);
        if (id == null)
        {
            ModelState.AddModelError(nameof(model.YouTubeUrl), "Invalid YouTube URL.");
            return View(model);
        }

        model.YouTubeId = id;
        _context.Videos.Add(model);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ========== EDIT (GET) ==========
    public async Task<IActionResult> Edit(int id)
    {
        var v = await _context.Videos.FindAsync(id);
        if (v == null) return NotFound();

        return View(v);
    }

    // ========== EDIT (POST) ==========
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Video model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var idExtract = YouTubeHelper.ExtractYouTubeId(model.YouTubeUrl);
        if (idExtract == null)
        {
            ModelState.AddModelError(nameof(model.YouTubeUrl), "Invalid YouTube URL.");
            return View(model);
        }

        var existing = await _context.Videos.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Title = model.Title;
        existing.YouTubeUrl = model.YouTubeUrl;
        existing.YouTubeId = idExtract;
        existing.SortOrder = model.SortOrder;

        _context.Update(existing);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ========== DELETE (GET) ==========
    public async Task<IActionResult> Delete(int id)
    {
        var video = await _context.Videos.FindAsync(id);
        if (video == null)
            return NotFound();

        return View(video);
    }

    // ========== DELETE (POST) CONFIRM ==========
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var video = await _context.Videos.FindAsync(id);
        if (video == null)
            return NotFound();

        _context.Videos.Remove(video);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
