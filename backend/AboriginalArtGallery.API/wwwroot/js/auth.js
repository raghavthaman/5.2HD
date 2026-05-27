// ═══════════════════════════════════════════════════════════
// AUTH
// ═══════════════════════════════════════════════════════════
function openModal(tab = "login") {
    document.getElementById("auth-modal").classList.add("open");
    switchTab(tab);
}
function closeModal() {
    document.getElementById("auth-modal").classList.remove("open");
}

function switchTab(tab) {
    document.getElementById("form-login").style.display = tab === "login" ? "" : "none";
    document.getElementById("form-register").style.display = tab === "register" ? "" : "none";
    document.getElementById("tab-login").classList.toggle("active", tab === "login");
    document.getElementById("tab-register").classList.toggle("active", tab === "register");
}

async function doLogin() {
    const email = document.getElementById("login-email").value.trim();
    const password = document.getElementById("login-password").value;
    document.getElementById("login-error").textContent = "";

    const r = await post("/api/auth/login", { email, password });
    if (!r.ok) {
        document.getElementById("login-error").textContent = r.data?.message || "Login failed";
        return;
    }

    state.token = r.data.token;
    state.user = { username: r.data.username, role: r.data.role };
    localStorage.setItem("gallery_token", state.token);
    localStorage.setItem("gallery_user", JSON.stringify(state.user));
    closeModal();
    renderNav();
    toast(`Welcome back, ${r.data.username}! 🎨`, "success");
}

async function doSendOtp() {
    const username = document.getElementById("reg-username").value.trim();
    const email = document.getElementById("reg-email").value.trim();
    const password = document.getElementById("reg-password").value;
    document.getElementById("reg-error").textContent = "";

    const r = await post("/api/auth/send-otp", { username, email, password });
    if (!r.ok) {
        document.getElementById("reg-error").textContent = r.data?.message || JSON.stringify(r.data);
        return;
    }
    
    // Move to step 2
    document.getElementById("reg-step1").style.display = "none";
    document.getElementById("reg-step2").style.display = "";
    document.getElementById("reg-otp-email").textContent = email;
    toast("OTP sent to your email!", "info");
}

async function doVerifyOtp() {
    const email = document.getElementById("reg-email").value.trim();
    const otp = document.getElementById("reg-otp").value.trim();
    document.getElementById("reg-otp-error").textContent = "";

    const r = await post("/api/auth/verify-otp", { email, otp });
    if (!r.ok) {
        document.getElementById("reg-otp-error").textContent = r.data?.message || JSON.stringify(r.data);
        return;
    }
    
    toast("Account created! Please log in.", "success");
    switchTab("login");
    document.getElementById("login-email").value = email;
    
    // Reset register form state
    document.getElementById("reg-step1").style.display = "";
    document.getElementById("reg-step2").style.display = "none";
    document.getElementById("reg-username").value = "";
    document.getElementById("reg-password").value = "";
    document.getElementById("reg-otp").value = "";
}

async function doResendOtp() {
    const username = document.getElementById("reg-username").value.trim();
    const email = document.getElementById("reg-email").value.trim();
    const password = document.getElementById("reg-password").value;
    
    const r = await post("/api/auth/send-otp", { username, email, password });
    if (r.ok) {
        toast("A new code has been sent.", "info");
    } else {
        document.getElementById("reg-otp-error").textContent = r.data?.message || "Failed to resend code.";
    }
}
function doLogout() {
    state.token = null;
    state.user = null;
    localStorage.removeItem("gallery_token");
    localStorage.removeItem("gallery_user");
    renderNav();
    showPage("home");
    toast("Logged out", "info");
}

function renderNav() {
    const el = document.getElementById("nav-auth");
    if (isLoggedIn()) {
        el.innerHTML = `
      <div class="user-pill">
        <div class="user-pill-avatar">${state.user.username[0].toUpperCase()}</div>
        <span class="user-pill-name">${state.user.username}</span>
        <span class="user-pill-role">${state.user.role}</span>
      </div>
      ${isAdmin() ? `<button class="btn btn-outline btn-sm" onclick="showPage('admin')">Admin</button>` : ""}
      <button class="btn btn-ghost btn-sm" onclick="doLogout()">Log out</button>
    `;
    } else {
        el.innerHTML = `
      <button class="btn btn-ghost btn-sm" onclick="openModal('login')">Log in</button>
      <button class="btn btn-primary btn-sm" onclick="openModal('register')">Register</button>
    `;
    }
}
