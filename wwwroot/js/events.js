/* ==========================================
   EVENTS.JS
   Countdown + Helpers
========================================== */

// Format number 0 → 00
function two(n) {
    return n < 10 ? "0" + n : n;
}

// Initialize countdown timer
function initCountdown(startDateIso, elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;

    const target = new Date(startDateIso);

    function updateCountdown() {
        const now = new Date();
        const diff = target - now;

        if (diff <= 0) {
            el.innerHTML = "<span class='text-success fw-bold'>Event Started</span>";
            return;
        }

        const days = Math.floor(diff / (1000 * 60 * 60 * 24));
        const hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
        const minutes = Math.floor((diff / (1000 * 60)) % 60);
        const seconds = Math.floor((diff / 1000) % 60);

        el.innerHTML =
            `<div class="countdown-box">
                ${days}d ${two(hours)}h ${two(minutes)}m ${two(seconds)}s
            </div>`;
    }

    // first run
    updateCountdown();

    // run every second
    setInterval(updateCountdown, 1000);
}


/* ==========================================
   OPTIONAL EXTRA UTILITIES
========================================== */

// Smooth scroll to top
function scrollToTop() {
    window.scrollTo({ top: 0, behavior: "smooth" });
}

// Highlight success messages
document.addEventListener("DOMContentLoaded", () => {
    const success = document.querySelector(".alert-success");
    if (success) {
        success.style.transition = "0.5s";
        setTimeout(() => success.style.opacity = "0.85", 300);
    }
});
