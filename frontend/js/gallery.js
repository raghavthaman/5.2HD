// ═══════════════════════════════════════════════════════════
// GALLERY & ARTISTS
// ═══════════════════════════════════════════════════════════

async function loadArtifacts(page = 1) {
    state.artifactPage = page;
    const search = document.getElementById("search-input").value;
    const type = document.getElementById("type-filter").value;

    document.getElementById("artifacts-grid").innerHTML =
        `<div class="loading"><div class="spinner"></div><p>Loading gallery...</p></div>`;

    let url = `/api/artifacts?page=${page}&pageSize=12`;
    if (search) url += `&search=${encodeURIComponent(search)}`;
    if (type)   url += `&artType=${encodeURIComponent(type)}`;

    const r = await get(url);
    if (!r.ok) {
        document.getElementById("artifacts-grid").innerHTML =
            `<div class="empty-state"><div class="empty-icon">⚠️</div><h3>Failed to load gallery</h3></div>`;
        return;
    }

    const countEl = document.getElementById("artifacts-count");
    if (countEl) countEl.textContent = `${r.data.total} artworks`;

    renderArtifactCards(r.data.items || [], "artifacts-grid");
}

function renderArtifactCards(items, containerId) {
    const c = document.getElementById(containerId);
    if (!items.length) {
        c.innerHTML = `
        <div class="empty-state" style="grid-column: 1/-1;">
            <div class="empty-icon">🏜️</div>
            <h3>No artworks found</h3>
            <p>Try adjusting your search filters.</p>
        </div>`;
        return;
    }

    c.innerHTML = items.map(a => `
        <div class="card" onclick="viewArtifact(${a.id})">
            <div class="card-img-wrap">
                ${a.imageUrl
                    ? `<img src="${a.imageUrl}" class="card-img" alt="${escHtml(a.title)}" loading="lazy">`
                    : `<div class="card-img card-img-placeholder">🎨</div>`}
                <div class="card-img-overlay">
                    <span class="tag-chip">${a.artType || 'Artwork'}</span>
                    ${a.yearCreated ? `<span class="tag-chip">${a.yearCreated}</span>` : ''}
                    ${a.isAvailableForPurchase && a.stockQuantity <= 0 ? `<span class="tag-chip" style="background:var(--red-clay);color:white;font-weight:bold;">OUT OF STOCK</span>` : ''}
                </div>
            </div>
            <div class="card-body">
                <h3 class="card-title">${escHtml(a.title)}</h3>
                <div class="card-meta">By <strong>${escHtml(a.artistName || 'Unknown Artist')}</strong></div>
                <p class="card-desc">${escHtml(a.description || '')}</p>
                <div class="card-footer">
                    ${a.isAvailableForPurchase
                        ? (a.stockQuantity <= 0
                            ? `<span class="card-price" style="text-decoration: line-through; opacity: 0.6;">$${Number(a.price).toFixed(2)} AUD</span>
                               <button class="btn btn-primary btn-sm" disabled onclick="event.stopPropagation();">Out of Stock</button>`
                            : `<span class="card-price">$${Number(a.price).toFixed(2)} AUD</span>
                               <button class="btn btn-primary btn-sm" onclick="event.stopPropagation(); addToCart({
                                   id: ${a.id}, title: '${escAttr(a.title)}', artistName: '${escAttr(a.artistName || '')}',
                                   price: ${a.price}, imageUrl: '${escAttr(a.imageUrl || '')}', isAvailableForPurchase: true, stockQuantity: ${a.stockQuantity}
                               })">Add to Cart</button>`
                          )
                        : `<span class="badge-not-for-sale">Not for Sale</span>
                           <button class="btn btn-outline btn-sm" onclick="event.stopPropagation(); viewArtifact(${a.id})">View Details</button>`
                    }
                </div>
            </div>
        </div>
    `).join('');
}

// Safe HTML escape helpers
function escHtml(str) {
    if (!str) return '';
    return String(str).replace(/&/g,'&amp;').replace(/</g,'&lt;').replace(/>/g,'&gt;').replace(/"/g,'&quot;');
}
function escAttr(str) {
    if (!str) return '';
    return String(str).replace(/'/g, "\\'").replace(/\n/g, ' ');
}

async function viewArtifact(id) {
    showPage("artifact-detail");
    const container = document.getElementById("detail-content");
    container.innerHTML = `<div class="loading"><div class="spinner"></div><p>Loading artwork details...</p></div>`;

    const r = await get(`/api/artifacts/${id}`);
    if (!r.ok) {
        container.innerHTML = `<div class="empty-state"><h3>Artwork not found</h3></div>`;
        return;
    }

    const a = r.data;
    container.innerHTML = `
    <div class="detail-grid">
        <div class="detail-img-wrap">
            ${a.imageUrl
                ? `<img src="${a.imageUrl}" class="detail-img" alt="${escHtml(a.title)}">`
                : `<div class="detail-img">🎨</div>`}
        </div>

        <div>
            <div class="detail-breadcrumb">${escHtml(a.artType || 'Artwork')}</div>
            <h1 class="detail-title">${escHtml(a.title)}</h1>

            <div class="detail-meta-row">
                <div>
                    <div class="detail-meta-label">Artist</div>
                    <div class="detail-meta-value">${escHtml(a.artistName || 'Unknown')}</div>
                </div>
                ${a.yearCreated ? `
                <div>
                    <div class="detail-meta-label">Year Created</div>
                    <div class="detail-meta-value">${a.yearCreated}</div>
                </div>` : ''}
                ${a.artType ? `
                <div>
                    <div class="detail-meta-label">Medium</div>
                    <div class="detail-meta-value">${escHtml(a.artType)}</div>
                </div>` : ''}
                ${a.tags && a.tags.length ? `
                <div>
                    <div class="detail-meta-label">Tags</div>
                    <div class="detail-meta-value">${a.tags.map(escHtml).join(', ')}</div>
                </div>` : ''}
            </div>

            <div class="detail-desc">${escHtml(a.description || '').replace(/\n/g,'<br>')}</div>

            <div class="detail-action-bar">
                ${a.isAvailableForPurchase
                    ? (a.stockQuantity <= 0
                        ? `<div class="detail-price" style="text-decoration: line-through; opacity: 0.6;">$${Number(a.price).toFixed(2)} AUD</div>
                           <span class="badge-out-of-stock" style="font-size:1rem;padding:0.5rem 1.2rem;">Out of Stock</span>
                           <button class="btn btn-primary" disabled>Out of Stock</button>`
                        : `<div class="detail-price">$${Number(a.price).toFixed(2)} AUD</div>
                           <button class="btn btn-primary" onclick="addToCart({
                               id: ${a.id}, title: '${escAttr(a.title)}', artistName: '${escAttr(a.artistName || '')}',
                               price: ${a.price}, imageUrl: '${escAttr(a.imageUrl || '')}', isAvailableForPurchase: true, stockQuantity: ${a.stockQuantity}
                           })">Add to Cart</button>`
                      )
                    : `<div style="display:flex;align-items:center;gap:1rem;">
                           <span class="badge-not-for-sale" style="font-size:1rem;padding:0.5rem 1.2rem;">Not for Sale</span>
                           <span style="font-size:0.85rem;color:#8a7060;">This artwork is on display only.</span>
                       </div>`
                }
            </div>

            <div class="comments-section">
                <h3 style="font-family:var(--font-display);font-size:1.3rem;margin-bottom:1rem;color:var(--earth);">Discussion</h3>
                ${isLoggedIn() ? `
                <div class="comment-form">
                    <textarea id="comment-input" class="input" placeholder="Share your thoughts about this artwork..."></textarea>
                    <button class="btn btn-primary" style="align-self:flex-start;" onclick="postComment(${a.id})">Post Comment</button>
                </div>` : `
                <div style="padding:1rem;background:var(--sand);border-radius:var(--radius);margin-bottom:1.5rem;font-size:0.9rem;">
                    Please <a href="#" onclick="openModal('login');return false;" style="color:var(--ochre);font-weight:600;">log in</a> to leave a comment.
                </div>`}
                <div id="comments-list">
                    ${a.comments && a.comments.length
                        ? a.comments.map(c => `
                            <div class="comment-item">
                                <div class="comment-header">
                                    <span class="comment-author">${escHtml(c.username || 'User')}</span>
                                    <span class="comment-date">${new Date(c.createdAt).toLocaleDateString()}</span>
                                </div>
                                <div class="comment-text">${escHtml(c.content)}</div>
                            </div>`).join('')
                        : `<p style="color:#a89880;font-size:0.9rem;">No comments yet. Be the first!</p>`
                    }
                </div>
            </div>
        </div>
    </div>
    `;
}

async function postComment(artifactId) {
    const content = document.getElementById("comment-input").value.trim();
    if (!content) return;
    const r = await post(`/api/comments`, { artifactId, content });
    if (!r.ok) { toast("Failed to post comment", "error"); return; }
    toast("Comment posted!", "success");
    viewArtifact(artifactId);
}

async function loadArtists() {
    const c = document.getElementById("artists-grid");
    c.innerHTML = `<div class="loading"><div class="spinner"></div><p>Loading artists...</p></div>`;

    const r = await get(`/api/artists`);
    if (!r.ok) { c.innerHTML = `<div class="empty-state"><h3>Failed to load</h3></div>`; return; }

    if (!r.data.length) {
        c.innerHTML = `<div class="empty-state"><h3>No artists found</h3></div>`;
        return;
    }

    c.innerHTML = r.data.map(a => `
    <div class="card artist-card" onclick="viewArtist(${a.id})">
        <div class="card-avatar">
            <div class="card-avatar-inner">${escHtml(a.name[0].toUpperCase())}</div>
        </div>
        <div class="card-body" style="text-align:center;">
            <h3 class="card-title">${escHtml(a.name)}</h3>
            <div class="card-meta" style="margin-bottom:0.5rem;">
                ${escHtml(a.tribe || 'Tribe unknown')} &bull; ${escHtml(a.country || 'Australia')}
                ${a.birthYear ? ` &bull; b.${a.birthYear}` : ''}
            </div>
            <p class="card-desc" style="margin-bottom:1.5rem;">${escHtml(a.biography || '')}</p>
            <button class="btn btn-outline btn-sm" style="width:100%;"
                onclick="event.stopPropagation(); viewArtist(${a.id})">View Profile</button>
        </div>
    </div>
    `).join('');
}

async function viewArtist(id) {
    showPage("artist-detail");
    const container = document.getElementById("artist-detail-content");
    container.innerHTML = `<div class="loading"><div class="spinner"></div><p>Loading profile...</p></div>`;

    const r = await get(`/api/artists/${id}`);
    if (!r.ok) { container.innerHTML = `<div class="empty-state"><h3>Artist not found</h3></div>`; return; }

    const a = r.data;
    container.innerHTML = `
    <div style="background:var(--white);padding:3rem;border-radius:var(--radius-lg);box-shadow:var(--shadow-lg);margin-bottom:3rem;">
        <div style="display:flex;align-items:center;gap:2rem;margin-bottom:2rem;border-bottom:1px solid rgba(201,123,58,0.15);padding-bottom:2rem;">
            <div style="width:100px;height:100px;border-radius:50%;background:var(--ochre);display:flex;align-items:center;justify-content:center;
                        font-family:var(--font-display);font-size:3rem;font-weight:800;color:var(--white);flex-shrink:0;">
                ${escHtml(a.name[0].toUpperCase())}
            </div>
            <div>
                <h1 style="font-family:var(--font-display);font-size:2.5rem;font-weight:800;color:var(--earth);margin-bottom:0.5rem;">${escHtml(a.name)}</h1>
                <div style="font-size:1rem;color:var(--ochre-dark);font-weight:500;letter-spacing:0.05em;text-transform:uppercase;">
                    ${escHtml(a.tribe || 'Tribe unknown')} &bull; ${escHtml(a.country || 'Australia')}
                </div>
                ${a.birthYear ? `<div style="font-size:0.9rem;color:#8a7060;margin-top:0.3rem;">Born ${a.birthYear}</div>` : ''}
            </div>
        </div>
        <h3 style="font-family:var(--font-display);font-size:1.3rem;margin-bottom:1rem;color:var(--earth);">Biography</h3>
        <div style="font-size:1.05rem;line-height:1.8;color:#4a3828;">${escHtml(a.biography || 'No biography available.').replace(/\n/g,'<br>')}</div>
    </div>

    <div class="section-header">
        <h2 class="section-title"><small>Portfolio</small> Artworks by ${escHtml(a.name)}</h2>
    </div>
    <div id="artist-artifacts-grid" class="grid"></div>
    `;

    if (a.artifacts && a.artifacts.length) {
        renderArtifactCards(a.artifacts.map(art => ({...art, artistName: a.name})), "artist-artifacts-grid");
    } else {
        document.getElementById("artist-artifacts-grid").innerHTML = `
        <div class="empty-state" style="grid-column:1/-1;">
            <div class="empty-icon">🖼️</div>
            <h3>No artworks yet</h3>
            <p>This artist's portfolio will appear here once artworks are added.</p>
        </div>`;
    }
}
