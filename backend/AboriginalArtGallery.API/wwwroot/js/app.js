// ═══════════════════════════════════════════════════════════
// ROUTING & INIT
// ═══════════════════════════════════════════════════════════
function showPage(pageId) {
    if (pageId === "admin" && !isAdmin()) {
        toast("Unauthorized. Admin access required.", "error");
        return;
    }

    document.querySelectorAll(".page").forEach((p) => p.classList.remove("active"));
    const p = document.getElementById(`page-${pageId}`);
    if (p) {
        p.classList.add("active");
        state.currentPage = pageId;
        window.scrollTo(0, 0);

        // Update nav buttons
        document.querySelectorAll(".nav-btn").forEach((b) => b.classList.remove("active"));
        const btn = document.getElementById(`nav-${pageId}`);
        if (btn) btn.classList.add("active");

        // Page specific logic
        if (pageId === "artifacts") loadArtifacts();
        if (pageId === "artists") loadArtists();
        if (pageId === "admin") {
            // Placeholder: loadAdminSection(state.adminSection);
            document.getElementById("admin-content-area").innerHTML = `<div class="empty-state"><h3>Admin Panel Active</h3><p>Use the sidebar to manage content.</p></div>`;
        }
        if (pageId === "cart") renderCartPage();
        if (pageId === "checkout") setCheckoutStep(1);
    }
}

// Ensure elements exist before adding listeners
document.addEventListener("DOMContentLoaded", () => {
    renderNav();
    updateCartBadge();
    
    // Add event listeners for enter key on search
    const searchInput = document.getElementById("search-input");
    if (searchInput) {
        searchInput.addEventListener("keypress", (e) => {
            if (e.key === "Enter") loadArtifacts(1);
        });
    }

    // Auth Modal tab switching listeners
    document.getElementById("tab-login")?.addEventListener("click", () => switchTab("login"));
    document.getElementById("tab-register")?.addEventListener("click", () => switchTab("register"));
});
