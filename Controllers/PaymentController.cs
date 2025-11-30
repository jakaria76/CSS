using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CSS.Data;
using CSS.Models;
using CSS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace CSS.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AamarPayService _aamar;
        private readonly AamarPaySettings _settings;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            ApplicationDbContext db,
            AamarPayService aamar,
            IOptions<AamarPaySettings> options,
            ILogger<PaymentController> logger)
        {
            _db = db;
            _aamar = aamar;
            _settings = options.Value;
            _logger = logger;
        }

        // =====================================================================
        // 1️⃣ REGISTER (FREE / PAID)
        // =====================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int eventId,
            string FullName,
            string Mobile,
            string? Email,
            string? Gender,
            string? InstituteName,
            string? ClassName,
            bool WillVolunteer,
            string? WhyJoin,
            string PaymentMethod,
            IFormFile? UserImage)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction("Index", "Events");
            }

            // Duplicate mobile check
            var exists = await _db.EventRegistrations
                .AnyAsync(r => r.EventId == eventId && r.Mobile == Mobile);

            if (exists)
            {
                TempData["Error"] = "This mobile number is already registered.";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // =============================== FREE EVENT ===============================
            if (ev.Price <= 0 || PaymentMethod == "Free")
            {
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
                    WillVolunteer = WillVolunteer,
                    PaymentMethod = "Free",
                    RegisteredAt = DateTime.UtcNow
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

                TempData["Registered"] = "Registration Successful!";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            // =============================== PAID EVENT ===============================

            // First: Pre-Registration (store image)
            var prereg = new EventRegistration
            {
                EventId = eventId,
                FullName = FullName,
                Mobile = Mobile,
                Email = Email,
                Gender = Gender,
                InstituteName = InstituteName,
                ClassName = ClassName,
                WhyJoin = WhyJoin,
                WillVolunteer = WillVolunteer,
                PaymentMethod = "Pending",
                RegisteredAt = DateTime.UtcNow
            };

            if (UserImage != null)
            {
                using var ms = new MemoryStream();
                await UserImage.CopyToAsync(ms);
                prereg.UserImage = ms.ToArray();
                prereg.UserImageType = UserImage.ContentType;
            }

            _db.EventRegistrations.Add(prereg);
            await _db.SaveChangesAsync();

            // Create Payment Transaction
            string tranId = $"EV{eventId}-{Guid.NewGuid():N}".Substring(0, 30);

            var tx = new PaymentTransaction
            {
                EventId = eventId,
                TranId = tranId,
                Amount = ev.Price,
                Currency = "BDT",
                PaymentGateway = "aamarpay",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,

                // store pre-reg inside payer meta
                PayerFullName = FullName,
                PayerMobile = Mobile,
                PayerEmail = Email,
                PayerDataJson = prereg.Id.ToString() // store prereg id as JSON
            };

            _db.PaymentTransactions.Add(tx);
            await _db.SaveChangesAsync();

            var successUrl = Url.Action("Success", "Payment", new { id = tx.Id }, Request.Scheme)!;
            var failUrl = Url.Action("Fail", "Payment", new { id = tx.Id }, Request.Scheme)!;
            var cancelUrl = Url.Action("Cancel", "Payment", new { id = tx.Id }, Request.Scheme)!;

            var resp = await _aamar.InitiatePaymentAsync(
                tranId, ev.Price, FullName, Mobile,
                Email ?? "test@test.com",
                $"Payment for {ev.Title}",
                successUrl, failUrl, cancelUrl
            );

            if (resp == null)
            {
                TempData["Error"] = "Payment gateway error!";
                return RedirectToAction("Details", "Events", new { id = eventId });
            }

            string redirectUrl =
                resp.Value.GetProperty("payment_url").GetString()
                ?? resp.Value.GetProperty("url").GetString();

            return Redirect(redirectUrl);
        }

        // =====================================================================
        // 2️⃣ SUCCESS
        // =====================================================================
        // =====================================================================
        // 2️⃣ PAYMENT SUCCESS
        // =====================================================================
        [HttpPost]
        public async Task<IActionResult> Success(int id)
        {
            var tx = await _db.PaymentTransactions.FindAsync(id);
            if (tx == null) return NotFound();

            tx.Status = "Success";
            tx.UpdatedAt = DateTime.UtcNow;

            // ⭐ prereg ID stored inside PayerDataJson
            int preregId = int.Parse(tx.PayerDataJson!);

            var prereg = await _db.EventRegistrations.FindAsync(preregId);
            if (prereg == null)
            {
                TempData["Error"] = "Pre-registration not found!";
                return RedirectToAction("Details", "Events", new { id = tx.EventId });
            }

            // ⭐ FINAL CONFIRMATION (IMAGE + DATA PRESERVED)
            prereg.PaymentMethod = "aamarpay";
            prereg.RegisteredAt = DateTime.UtcNow;

            // ⭐ VERY IMPORTANT: Image stays same (DO NOT REMOVE THIS!)
            // prereg.UserImage already contains uploaded image
            // prereg.UserImageType already contains correct mime type

            // link to transaction
            tx.RegistrationId = prereg.Id;

            await _db.SaveChangesAsync();

            TempData["Registered"] = "🎉 Payment successful! Registration completed.";

            return RedirectToAction("Details", "Events", new { id = tx.EventId });
        }


        // =====================================================================
        // 3️⃣ FAIL
        // =====================================================================
        [HttpPost]
        public async Task<IActionResult> Fail(int id)
        {
            var tx = await _db.PaymentTransactions.FindAsync(id);
            if (tx != null)
            {
                tx.Status = "Failed";
                await _db.SaveChangesAsync();
            }

            TempData["Error"] = "❌ Payment Failed!";
            return RedirectToAction("Details", "Events", new { id = tx?.EventId });
        }

        // =====================================================================
        // 4️⃣ CANCEL
        // =====================================================================
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var tx = await _db.PaymentTransactions.FindAsync(id);
            if (tx != null)
            {
                tx.Status = "Cancelled";
                await _db.SaveChangesAsync();
            }

            TempData["Error"] = "⚠ Payment Cancelled!";
            return RedirectToAction("Details", "Events", new { id = tx?.EventId });
        }
    }
}
