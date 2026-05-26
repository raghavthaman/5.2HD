// ═══════════════════════════════════════════════════════════
// ADMIN PANEL
// ═══════════════════════════════════════════════════════════

async function loadAdminSection(section) {
    state.adminSection = section;
    document.getElementById("admin-title").textContent =
        section === "artworks" ? "Manage Artworks"
        : section === "artists" ? "Manage Artists"
        : section === "orders"  ? "All Orders"
        : "Admin Dashboard";

    document.querySelectorAll(".admin-nav-item").forEach(el => el.classList.remove("active"));
    const activeNav = document.querySelector(`.admin-nav-item[data-section="${section}"]`);
    if (activeNav) activeNav.classList.add("active");

    const area = document.getElementById("admin-content-area");
    area.innerHTML = `<div class="loading"><div class="spinner"></div><p>Loading...</p></div>`;

    if (section === "artworks") await loadAdminArtworks();
    else if (section === "artists") await loadAdminArtists();
    else if (section === "orders")  await loadAdminOrders();
}

// ─── ARTWORKS MANAGEMENT ──────────────────────────────────────────────────
async function loadAdminArtworks() {
    const r = await get("/api/artifacts?page=1&pageSize=100");
    if (!r.ok) {
        document.getElementById("admin-content-area").innerHTML =
            `<div class="empty-state"><h3>Failed to load artworks</h3></div>`;
        return;
    }

    const items = r.data.items || [];
    document.getElementById("admin-content-area").innerHTML = `
    <div class="admin-artworks">
        <div class="admin-artworks-header">
            <div style="display:flex; justify-content:space-between; align-items:center;">
                <p style="color:#8a7060;font-size:0.9rem;">${items.length} artworks total — toggle the switch to list an artwork for sale.</p>
                <button class="btn btn-primary btn-sm" onclick="showAddArtworkModal()">+ Add Artwork</button>
            </div>
        </div>
        <div class="admin-table-wrap">
            <table class="admin-table" id="artworks-table">
                <thead>
                    <tr>
                        <th style="width:80px">Thumb</th>
                        <th>Title &amp; Artist</th>
                        <th style="width:140px">Price (AUD)</th>
                        <th style="width:100px">Stock</th>
                        <th style="width:160px">Sale Status</th>
                    </tr>
                </thead>
                <tbody>
                    ${items.map(a => `
                    <tr id="row-${a.id}">
                        <td>
                            <div class="admin-thumb-wrap">
                                ${a.imageUrl
                                    ? `<img src="${a.imageUrl}" class="admin-thumb" alt="${escHtml(a.title)}" loading="lazy">`
                                    : `<div class="admin-thumb admin-thumb-placeholder">🎨</div>`}
                            </div>
                        </td>
                        <td>
                            <div class="admin-artifact-title">${escHtml(a.title)}</div>
                            <div class="admin-artifact-artist">${escHtml(a.artistName || 'Unknown')}</div>
                            <div style="font-size:0.75rem;color:#b0a090;margin-top:0.2rem;">${escHtml(a.artType || '')} ${a.yearCreated ? '· ' + a.yearCreated : ''}</div>
                        </td>
                        <td>
                            <input
                                type="number"
                                id="price-${a.id}"
                                class="input admin-price-input"
                                value="${Number(a.price || 0).toFixed(2)}"
                                min="0"
                                step="0.01"
                                ${!a.isAvailableForPurchase ? 'disabled' : ''}
                                onchange="onPriceChange(${a.id})"
                            >
                        </td>
                        <td>
                            <input
                                type="number"
                                id="stock-${a.id}"
                                class="input admin-price-input"
                                value="${a.stockQuantity || 0}"
                                min="0"
                                onchange="onPriceChange(${a.id})"
                            >
                        </td>
                        <td>
                            <div class="sale-toggle-wrap">
                                <label class="sale-toggle" title="${a.isAvailableForPurchase ? 'Click to remove from sale' : 'Click to list for sale'}">
                                    <input
                                        type="checkbox"
                                        id="toggle-${a.id}"
                                        ${a.isAvailableForPurchase ? 'checked' : ''}
                                        onchange="onSaleToggle(${a.id})"
                                    >
                                    <span class="sale-slider"></span>
                                </label>
                                <span class="sale-label" id="sale-label-${a.id}">
                                    ${a.isAvailableForPurchase
                                        ? `<span style="color:var(--sage);font-weight:600;">For Sale</span>`
                                        : `<span style="color:#a89880;">Not for Sale</span>`}
                                </span>
                            </div>
                        </td>
                    </tr>`).join('')}
                </tbody>
            </table>
        </div>
    </div>`;
}

async function onSaleToggle(id) {
    const toggle = document.getElementById(`toggle-${id}`);
    const priceInput = document.getElementById(`price-${id}`);
    const stockInput = document.getElementById(`stock-${id}`);
    const labelEl = document.getElementById(`sale-label-${id}`);
    
    const isForSale = toggle.checked;
    const price = parseFloat(priceInput.value) || 0;
    const stock = parseInt(stockInput.value) || 0;

    // Optimistic UI update
    priceInput.disabled = !isForSale;
    labelEl.innerHTML = isForSale
        ? `<span style="color:var(--sage);font-weight:600;">For Sale</span>`
        : `<span style="color:#a89880;">Not for Sale</span>`;

    const r = await patch(`/api/artifacts/${id}/sale-status`, {
        isAvailableForPurchase: isForSale,
        price: isForSale ? price : 0,
        stockQuantity: stock
    });

    if (!r.ok) {
        // Revert on failure
        toggle.checked = !isForSale;
        priceInput.disabled = isForSale;
        labelEl.innerHTML = !isForSale
            ? `<span style="color:var(--sage);font-weight:600;">For Sale</span>`
            : `<span style="color:#a89880;">Not for Sale</span>`;
        toast("Failed to update sale status", "error");
        return;
    }

    toast(isForSale ? `Listed for $${price.toFixed(2)} AUD` : `Removed from sale`, "success");
}

function onPriceChange(id) {
    const toggle = document.getElementById(`toggle-${id}`);
    if (toggle.checked) {
        // Auto-save when price or stock changes while item is for sale
        onSaleToggle(id);
    } else {
        // Just save stock if it's not for sale
        onSaleToggle(id);
    }
}

// ─── ADMIN MODAL LOGIC ────────────────────────────────────────────────────
function closeAdminModal(id) {
    document.getElementById(id).style.display = 'none';
}

async function showAddArtworkModal() {
    const r = await get("/api/artists");
    if (r.ok && r.data) {
        const select = document.getElementById("add-art-artist");
        select.innerHTML = r.data.map(a => `<option value="${a.id}">${escHtml(a.name)}</option>`).join("");
    }
    document.getElementById("add-artwork-modal").style.display = "flex";
}

async function submitAddArtwork() {
    const title = document.getElementById("add-art-title").value.trim();
    if (!title) return toast("Title is required", "error");

    const payload = {
        title: title,
        artistId: parseInt(document.getElementById("add-art-artist").value),
        artType: document.getElementById("add-art-type").value.trim() || null,
        yearCreated: parseInt(document.getElementById("add-art-year").value) || null,
        price: parseFloat(document.getElementById("add-art-price").value) || 0,
        stockQuantity: parseInt(document.getElementById("add-art-stock").value) || 1,
        imageUrl: document.getElementById("add-art-img").value.trim() || null,
        isAvailableForPurchase: true
    };

    const r = await post("/api/artifacts", payload);
    if (r.ok) {
        toast("Artwork added successfully!", "success");
        closeAdminModal("add-artwork-modal");
        loadAdminArtworks();
    } else {
        toast("Failed to add artwork", "error");
    }
}

// ─── ARTISTS MANAGEMENT ───────────────────────────────────────────────────
async function loadAdminArtists() {
    const r = await get("/api/artists");
    if (!r.ok) {
        document.getElementById("admin-content-area").innerHTML =
            `<div class="empty-state"><h3>Failed to load artists</h3></div>`;
        return;
    }

    const artists = r.data || [];
    document.getElementById("admin-content-area").innerHTML = `
    <div class="admin-table-wrap">
        <div style="display:flex; justify-content:flex-end; padding:1rem;">
            <button class="btn btn-primary btn-sm" onclick="showAddArtistModal()">+ Add Artist</button>
        </div>
        <table class="admin-table">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Tribe</th>
                    <th>Country</th>
                    <th>Birth Year</th>
                    <th style="width:80px">Actions</th>
                </tr>
            </thead>
            <tbody>
                ${artists.map(a => `
                <tr>
                    <td style="font-weight:600;">${escHtml(a.name)}</td>
                    <td>${escHtml(a.tribe || '—')}</td>
                    <td>${escHtml(a.country || '—')}</td>
                    <td>${a.birthYear || '—'}</td>
                    <td>
                        <button class="btn btn-danger btn-sm" onclick="deleteArtist(${a.id}, '${escAttr(a.name)}')">Delete</button>
                    </td>
                </tr>`).join('')}
            </tbody>
        </table>
    </div>`;
}

async function deleteArtist(id, name) {
    if (!confirm(`Delete artist "${name}"? This cannot be undone.`)) return;
    const r = await del(`/api/artists/${id}`);
    if (r.ok) { toast(`${name} deleted`, "success"); loadAdminArtists(); }
    else toast("Failed to delete artist", "error");
}

function showAddArtistModal() {
    document.getElementById("add-artist-modal").style.display = "flex";
}

async function submitAddArtist() {
    const name = document.getElementById("add-artist-name").value.trim();
    if (!name) return toast("Name is required", "error");

    const payload = {
        name: name,
        tribe: document.getElementById("add-artist-tribe").value.trim() || null,
        country: document.getElementById("add-artist-country").value.trim() || null,
        birthYear: parseInt(document.getElementById("add-artist-birth").value) || null
    };

    const r = await post("/api/artists", payload);
    if (r.ok) {
        toast("Artist added successfully!", "success");
        closeAdminModal("add-artist-modal");
        loadAdminArtists();
    } else {
        toast("Failed to add artist", "error");
    }
}

// ─── ORDERS MANAGEMENT ────────────────────────────────────────────────────
async function loadAdminOrders() {
    const r = await get("/api/orders");
    if (!r.ok) {
        document.getElementById("admin-content-area").innerHTML =
            `<div class="empty-state"><h3>No orders yet</h3></div>`;
        return;
    }

    const orders = r.data || [];
    if (!orders.length) {
        document.getElementById("admin-content-area").innerHTML =
            `<div class="empty-state"><div class="empty-icon">📦</div><h3>No orders placed yet</h3></div>`;
        return;
    }

    document.getElementById("admin-content-area").innerHTML = `
    <div class="admin-table-wrap">
        <table class="admin-table">
            <thead>
                <tr>
                    <th>#</th>
                    <th>Customer</th>
                    <th>Items</th>
                    <th>Total</th>
                    <th>Status</th>
                    <th>Date</th>
                </tr>
            </thead>
            <tbody>
                ${orders.map(o => `
                <tr>
                    <td>#${o.id}</td>
                    <td>${escHtml(o.customerName || o.userId || '—')}</td>
                    <td>${o.orderItems ? o.orderItems.length : '—'}</td>
                    <td>$${Number(o.totalAmount || 0).toFixed(2)}</td>
                    <td><span class="tag-chip">${escHtml(o.status || 'Pending')}</span></td>
                    <td>${new Date(o.createdAt).toLocaleDateString()}</td>
                </tr>`).join('')}
            </tbody>
        </table>
    </div>`;
}
