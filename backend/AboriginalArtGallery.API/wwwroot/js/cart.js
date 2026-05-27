// ═══════════════════════════════════════════════════════════
// CART
// ═══════════════════════════════════════════════════════════
let cart = JSON.parse(localStorage.getItem("gallery_cart") || "[]");

function saveCart() {
    localStorage.setItem("gallery_cart", JSON.stringify(cart));
    updateCartBadge();
}

function updateCartBadge() {
    const el = document.getElementById("cart-badge");
    if (el) {
        const count = cart.reduce((sum, item) => sum + item.quantity, 0);
        el.textContent = count;
        el.style.display = count > 0 ? "block" : "none";
    }
}

function addToCart(artifact) {
    if (!artifact.isAvailableForPurchase) {
        toast("This artwork is not for sale.", "error");
        return;
    }
    if (artifact.stockQuantity !== undefined && artifact.stockQuantity <= 0) {
        toast("This artwork is out of stock.", "error");
        return;
    }
    const existing = cart.find(item => item.id === artifact.id);
    if (existing) {
        if (artifact.stockQuantity !== undefined && existing.quantity >= artifact.stockQuantity) {
            toast(`Cannot add more. Only ${artifact.stockQuantity} items in stock.`, "error");
            return;
        }
        existing.quantity += 1;
    } else {
        cart.push({
            id: artifact.id,
            title: artifact.title,
            artistName: artifact.artistName,
            price: artifact.price,
            imageUrl: artifact.imageUrl,
            quantity: 1,
            stockQuantity: artifact.stockQuantity
        });
    }
    saveCart();
    toast(`${artifact.title} added to cart!`, "success");
}

function updateQuantity(id, delta) {
    const item = cart.find(i => i.id === id);
    if (item) {
        if (delta > 0 && item.stockQuantity !== undefined && item.quantity + delta > item.stockQuantity) {
            toast(`Cannot increase quantity. Only ${item.stockQuantity} items in stock.`, "error");
            return;
        }
        item.quantity += delta;
        if (item.quantity <= 0) {
            cart = cart.filter(i => i.id !== id);
        }
        saveCart();
        renderCartPage();
    }
}

function renderCartPage() {
    const container = document.getElementById("cart-items-container");
    const summaryContainer = document.getElementById("cart-summary");
    
    if (!container || !summaryContainer) return;

    if (cart.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <div class="empty-icon">🛒</div>
                <h3>Your cart is empty</h3>
                <p>Looks like you haven't added any artworks yet.</p>
                <button class="btn btn-primary" style="margin-top: 1rem;" onclick="showPage('artifacts')">Explore Artworks</button>
            </div>
        `;
        summaryContainer.style.display = "none";
        return;
    }

    summaryContainer.style.display = "block";
    
    let subtotal = 0;
    container.innerHTML = cart.map(item => {
        const lineTotal = item.price * item.quantity;
        subtotal += lineTotal;
        return `
        <div class="cart-item">
            ${item.imageUrl ? `<img src="${item.imageUrl}" class="cart-item-img" alt="${item.title}">` : `<div class="cart-item-img" style="display:flex;align-items:center;justify-content:center;font-size:2rem;color:var(--ochre);">🎨</div>`}
            <div class="cart-item-info">
                <div class="cart-item-title">${item.title}</div>
                <div style="font-size:0.85rem;color:var(--earth-muted);">${item.artistName || "Unknown Artist"}</div>
                <div class="cart-item-price">$${item.price.toFixed(2)} AUD</div>
            </div>
            <div class="cart-item-controls">
                <div class="quantity-control">
                    <button class="quantity-btn" onclick="updateQuantity(${item.id}, -1)">-</button>
                    <span style="font-weight:600;min-width:20px;text-align:center;">${item.quantity}</span>
                    <button class="quantity-btn" onclick="updateQuantity(${item.id}, 1)">+</button>
                </div>
            </div>
        </div>
        `;
    }).join("");

    const shipping = subtotal > 500 ? 0 : 25;
    const tax = subtotal * 0.10;
    const total = subtotal + shipping + tax;

    summaryContainer.innerHTML = `
        <h3 style="font-family:var(--font-display);font-size:1.3rem;margin-bottom:1.5rem;color:var(--earth);">Order Summary</h3>
        <div class="summary-row">
            <span>Subtotal</span>
            <span style="font-weight:500;color:var(--earth);">$${subtotal.toFixed(2)}</span>
        </div>
        <div class="summary-row">
            <span>Shipping</span>
            <span style="font-weight:500;color:var(--earth);">${shipping === 0 ? "FREE" : `$${shipping.toFixed(2)}`}</span>
        </div>
        <div class="summary-row">
            <span>GST (10%)</span>
            <span style="font-weight:500;color:var(--earth);">$${tax.toFixed(2)}</span>
        </div>
        <div class="summary-total">
            <span>Total</span>
            <span style="color:var(--ochre-dark);">$${total.toFixed(2)} AUD</span>
        </div>
        <button class="btn btn-primary" style="width:100%;margin-top:2rem;padding:0.8rem;" onclick="goToCheckout()">Proceed to Checkout</button>
    `;
}

function goToCheckout() {
    if (!isLoggedIn()) {
        toast("Please log in to proceed to checkout.", "info");
        openModal('login');
        return;
    }
    showPage('checkout');
}
