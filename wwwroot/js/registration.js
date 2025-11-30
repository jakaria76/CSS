// ================= PAYMENT =================
function payNow() {
    document.getElementById("payMethod").value = "aamarpay";
    document.getElementById("regForm").submit();
}

// ================= FORM VALIDATION =================
function validateForm() {

    var mobile = document.querySelector('input[name="Mobile"]').value.trim();
    var email = document.querySelector('input[name="Email"]').value.trim();

    // Bangladesh mobile format: 01XXXXXXXXX
    var mobileRegex = /^01[0-9]{9}$/;
    if (!mobileRegex.test(mobile)) {
        alert("Please enter a valid Bangladesh mobile number (11 digits, starts with 01).");
        return false;
    }

    // Email format (optional)
    if (email.length > 0) {
        var emailRegex = /^[^@\s]+@[^@\s]+\.[^@\s]+$/;
        if (!emailRegex.test(email)) {
            alert("Invalid email format.");
            return false;
        }
    }

    return true;
}

// ================= AUTO-DISMISS SUCCESS MESSAGE =================
document.addEventListener("DOMContentLoaded", function () {
    var alertEl = document.getElementById("regSuccessAlert");
    if (!alertEl) return;

    setTimeout(function () {
        alertEl.scrollIntoView({ behavior: "smooth", block: "center" });
    }, 150);

    setTimeout(function () {
        try {
            var bsAlert = bootstrap.Alert.getOrCreateInstance(alertEl);
            bsAlert.close();
        } catch (e) {
            alertEl.remove();
        }
    }, 4000);
});

// ================= GOOGLE MAP =================

// ================= GOOGLE MAP =================
document.addEventListener("DOMContentLoaded", function () {

    var lat = "@Model.Event.Latitude";
    var lng = "@Model.Event.Longitude";

    // Check if no lat/lng exists
    if (!lat || !lng || lat === "null" || lng === "null" || lat === "" || lng === "") {
        document.getElementById("mapDetail").innerHTML =
            "<div class='text-muted text-center p-3'>Location not set</div>";
        return;
    }

    // Load Google Map iframe
    var mapFrame = `
        <iframe
            width="100%"
            height="220"
            style="border:0;border-radius:8px;"
            loading="lazy"
            allowfullscreen
            referrerpolicy="no-referrer-when-downgrade"
            src="https://www.google.com/maps?q=${lat},${lng}&z=16&output=embed">
        </iframe>
    `;

    document.getElementById("mapDetail").innerHTML = mapFrame;

});