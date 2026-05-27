// ═══════════════════════════════════════════════════════════
// API HELPERS
// ═══════════════════════════════════════════════════════════
const API = "";

let state = {
    token: localStorage.getItem("gallery_token"),
    user: JSON.parse(localStorage.getItem("gallery_user") || "null"),
    currentPage: "home",
    artifactPage: 1,
    artistPage: 1,
    adminSection: "artists",
};

async function api(path, opts = {}) {
    const headers = {
        "Content-Type": "application/json",
        ...(opts.headers || {}),
    };
    if (state.token) headers["Authorization"] = `Bearer ${state.token}`;
    const res = await fetch(`${API}${path}`, { ...opts, headers });
    const data = await res.json().catch(() => ({}));
    return { ok: res.ok, status: res.status, data };
}

const get   = (path)        => api(path, { method: "GET" });
const post  = (path, body)  => api(path, { method: "POST",  body: JSON.stringify(body) });
const put   = (path, body)  => api(path, { method: "PUT",   body: JSON.stringify(body) });
const patch = (path, body)  => api(path, { method: "PATCH", body: JSON.stringify(body) });
const del   = (path)        => api(path, { method: "DELETE" });

function toast(msg, type = "info") {
    const el = document.createElement("div");
    el.className = `toast-item toast-${type}`;
    el.textContent = msg;
    document.getElementById("toast").appendChild(el);
    setTimeout(() => el.remove(), 3500);
}

const isLoggedIn = () => !!state.token;
const isAdmin = () => state.user?.role === "Admin";
