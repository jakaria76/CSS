// ===============================
// PUSH EVENT
// ===============================
self.addEventListener("push", function (event) {
    console.log("🔥 PUSH EVENT TRIGGERED:", event);

    if (!event.data) {
        console.log("⚠ No data in push event");
        return;
    }

    let payload;
    try {
        payload = event.data.json();
    } catch (err) {
        console.error("❌ Invalid push payload JSON:", err);
        return;
    }

    console.log("🔥 PUSH PAYLOAD:", payload);

    const title = payload.title || "CSS Notification";

    const options = {
        body: payload.body || "You have a new message.",
        icon: "/images/csslogo.png",     // Main icon
        badge: "/images/csslogo.png",    // Small badge icon
        data: {
            url: payload.url || "/"
        },
        vibrate: [200, 100, 200]
    };

    event.waitUntil(
        self.registration.showNotification(title, options)
    );
});

// ===============================
// NOTIFICATION CLICK HANDLER
// ===============================
self.addEventListener("notificationclick", function (event) {
    event.notification.close();

    const targetUrl = event.notification.data?.url || "/";

    event.waitUntil(
        clients.matchAll({ type: "window", includeUncontrolled: true })
            .then(clientsList => {
                for (const client of clientsList) {
                    if (client.url.includes(self.location.origin)) {
                        return client.focus();
                    }
                }
                return clients.openWindow(targetUrl);
            })
    );
});
