// ═══════════════════════════════════════════════════════════
// CHECKOUT
// ═══════════════════════════════════════════════════════════
let checkoutState = {
    step: 1,
    delivery: {
        sameAsBilling: false,
        date: '',
        timeSlot: ''
    },
    gift: {
        isGift: false,
        message: '',
        style: '',
        couponCode: '',
        redeemCode: '',
        discountAmount: 0,
        discountType: ''
    },
    payment: {
        method: 'card',
        cardLastFour: '',
        cardBrand: ''
    }
};

function setCheckoutStep(step) {
    if (step > 4 || step < 1) return;
    
    // Validation before moving forward
    if (step > checkoutState.step) {
        if (checkoutState.step === 1 && !validateStep1()) return;
        if (checkoutState.step === 3 && !validateStep3()) return;
    }

    checkoutState.step = step;
    
    // Update UI Progress
    document.querySelectorAll('.checkout-step').forEach((el, idx) => {
        const s = idx + 1;
        el.classList.remove('active', 'completed');
        if (s === step) el.classList.add('active');
        if (s < step) el.classList.add('completed');
    });

    // Show Panels
    document.querySelectorAll('.checkout-panel').forEach((el, idx) => {
        el.classList.toggle('active', (idx + 1) === step);
    });

    if (step === 4) {
        renderOrderSummary();
    }
}

function toggleSameAsBilling() {
    const isSame = document.getElementById('chk-same-billing').checked;
    checkoutState.delivery.sameAsBilling = isSame;
    document.getElementById('delivery-address-form').style.display = isSame ? 'none' : 'block';
}

function toggleGiftOptions() {
    const isGift = document.getElementById('chk-is-gift').checked;
    checkoutState.gift.isGift = isGift;
    document.getElementById('gift-options-container').style.display = isGift ? 'block' : 'none';
    if (!isGift) checkoutState.gift.style = '';
}

function selectGiftWrap(style) {
    checkoutState.gift.style = style;
    document.querySelectorAll('.wrap-card').forEach(el => {
        el.classList.toggle('selected', el.dataset.style === style);
    });
}

function selectPaymentMethod(method) {
    checkoutState.payment.method = method;
    document.querySelectorAll('.payment-card').forEach(el => {
        el.classList.toggle('selected', el.dataset.method === method);
    });
    document.getElementById('card-details-form').style.display = method === 'card' ? 'block' : 'none';
}

function validateStep1() {
    const required = ['billing-fn', 'billing-ln', 'billing-a1', 'billing-city', 'billing-state', 'billing-pc', 'billing-phone'];
    if (!checkoutState.delivery.sameAsBilling) {
        required.push('delivery-fn', 'delivery-ln', 'delivery-a1', 'delivery-city', 'delivery-state', 'delivery-pc');
    }
    required.push('delivery-date', 'delivery-time');

    for (let id of required) {
        const val = document.getElementById(id).value.trim();
        if (!val) {
            toast("Please fill all required delivery fields.", "error");
            document.getElementById(id).focus();
            return false;
        }
    }
    
    checkoutState.delivery.date = document.getElementById('delivery-date').value;
    checkoutState.delivery.timeSlot = document.getElementById('delivery-time').value;
    return true;
}

function validateStep3() {
    if (checkoutState.payment.method === 'card') {
        const cardNum = document.getElementById('card-num').value.replace(/\s/g, '');
        if (cardNum.length < 15) {
            toast("Please enter a valid card number.", "error");
            return false;
        }
        checkoutState.payment.cardLastFour = cardNum.slice(-4);
        checkoutState.payment.cardBrand = cardNum.startsWith('4') ? 'Visa' : cardNum.startsWith('5') ? 'Mastercard' : 'Amex';
    }
    return true;
}

async function applyCoupon() {
    const code = document.getElementById('coupon-code').value.trim();
    if (!code) return;

    const r = await post("/api/promocodes/validate", { code });
    if (!r.ok) {
        toast(r.data?.message || "Invalid coupon code", "error");
        checkoutState.gift.couponCode = '';
        checkoutState.gift.discountAmount = 0;
        return;
    }

    checkoutState.gift.couponCode = r.data.code;
    checkoutState.gift.discountType = r.data.type;
    // We store the raw value here; it will be calculated properly in the summary
    checkoutState.gift.discountAmount = r.data.value; 
    
    toast(`Coupon ${code} applied successfully!`, "success");
}

function applyRedeem() {
    const code = document.getElementById('redeem-code').value.trim();
    if (!code) return;

    if (!/^GC-[A-Z0-9]{8}$/.test(code)) {
        toast("Invalid gift card format. Expected GC-XXXXXXXX", "error");
        return;
    }

    checkoutState.gift.redeemCode = code;
    toast(`Gift card ${code} applied! ($50 off)`, "success");
}

function formatCardNumber(e) {
    let val = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
    let matches = val.match(/\d{4,16}/g);
    let match = matches && matches[0] || '';
    let parts = [];
    for (let i = 0, len = match.length; i < len; i += 4) {
        parts.push(match.substring(i, i + 4));
    }
    if (parts.length) {
        e.target.value = parts.join(' ');
    } else {
        e.target.value = val;
    }
}

function renderOrderSummary() {
    const container = document.getElementById('checkout-order-summary');
    if (!container) return;

    let subtotal = 0;
    const itemsHtml = cart.map(item => {
        const lineTotal = item.price * item.quantity;
        subtotal += lineTotal;
        return `
            <div style="display:flex; justify-content:space-between; margin-bottom:0.5rem; font-size:0.9rem;">
                <div>${item.quantity}x ${item.title}</div>
                <div style="font-weight:600;">$${lineTotal.toFixed(2)}</div>
            </div>
        `;
    }).join("");

    let discount = 0;
    if (checkoutState.gift.couponCode) {
        if (checkoutState.gift.discountType === 'percentage') {
            discount += subtotal * (checkoutState.gift.discountAmount / 100);
        } else {
            discount += checkoutState.gift.discountAmount;
        }
    }
    if (checkoutState.gift.redeemCode) {
        discount += 50; // hardcoded dummy logic for gift card
    }
    if (discount > subtotal) discount = subtotal;

    const discountedSubtotal = subtotal - discount;
    const shipping = discountedSubtotal > 500 ? 0 : 25;
    
    let giftFee = 0;
    if (checkoutState.gift.isGift) {
        if (checkoutState.gift.style === 'luxury') giftFee = 30;
        else if (checkoutState.gift.style === 'premium') giftFee = 15;
        else if (checkoutState.gift.style === 'standard') giftFee = 5;
    }

    const tax = discountedSubtotal * 0.10;
    const total = discountedSubtotal + shipping + giftFee + tax;

    container.innerHTML = `
        <div style="background:var(--white); padding:2rem; border-radius:var(--radius-lg); border:1px solid rgba(201,123,58,0.12);">
            <h3 style="font-family:var(--font-display);font-size:1.3rem;margin-bottom:1.5rem;">Review Your Order</h3>
            
            <div style="margin-bottom:1.5rem; padding-bottom:1.5rem; border-bottom:1px solid #e8d8c4;">
                <h4 style="font-size:0.85rem; color:#8a7060; text-transform:uppercase; margin-bottom:0.5rem;">Items</h4>
                ${itemsHtml}
            </div>

            <div class="summary-row"><span>Subtotal</span><span>$${subtotal.toFixed(2)}</span></div>
            ${discount > 0 ? `<div class="summary-row" style="color:var(--sage);font-weight:600;"><span>Discount</span><span>-$${discount.toFixed(2)}</span></div>` : ''}
            <div class="summary-row"><span>Shipping</span><span>${shipping === 0 ? "FREE" : `$${shipping.toFixed(2)}`}</span></div>
            ${giftFee > 0 ? `<div class="summary-row"><span>Gift Wrap</span><span>$${giftFee.toFixed(2)}</span></div>` : ''}
            <div class="summary-row"><span>GST (10%)</span><span>$${tax.toFixed(2)}</span></div>
            
            <div class="summary-total" style="font-size:1.5rem; color:var(--ochre-dark);">
                <span>Total</span>
                <span>$${total.toFixed(2)} AUD</span>
            </div>
            
            <button class="btn btn-primary" style="width:100%; margin-top:2rem; padding:1rem; font-size:1.1rem;" onclick="placeOrder()">Place Order Securely</button>
        </div>
    `;
}

async function placeOrder() {
    const dto = {
        BillingFirstName: document.getElementById('billing-fn').value.trim(),
        BillingLastName: document.getElementById('billing-ln').value.trim(),
        BillingAddressLine1: document.getElementById('billing-a1').value.trim(),
        BillingAddressLine2: document.getElementById('billing-a2').value.trim(),
        BillingCity: document.getElementById('billing-city').value.trim(),
        BillingState: document.getElementById('billing-state').value.trim(),
        BillingPostCode: document.getElementById('billing-pc').value.trim(),
        BillingCountry: 'Australia',
        BillingPhone: document.getElementById('billing-phone').value.trim(),
        
        SameAsDelivery: checkoutState.delivery.sameAsBilling,
        DeliveryFirstName: document.getElementById('delivery-fn').value.trim(),
        DeliveryLastName: document.getElementById('delivery-ln').value.trim(),
        DeliveryAddressLine1: document.getElementById('delivery-a1').value.trim(),
        DeliveryAddressLine2: document.getElementById('delivery-a2').value.trim(),
        DeliveryCity: document.getElementById('delivery-city').value.trim(),
        DeliveryState: document.getElementById('delivery-state').value.trim(),
        DeliveryPostCode: document.getElementById('delivery-pc').value.trim(),
        DeliveryCountry: 'Australia',

        PreferredDeliveryDate: new Date(checkoutState.delivery.date).toISOString(),
        DeliveryTimeSlot: checkoutState.delivery.timeSlot,

        IsGift: checkoutState.gift.isGift,
        GiftMessage: document.getElementById('gift-msg').value.trim(),
        GiftWrapStyle: checkoutState.gift.style,

        CouponCode: checkoutState.gift.couponCode,
        RedeemCode: checkoutState.gift.redeemCode,

        PaymentMethod: checkoutState.payment.method,
        CardLastFour: checkoutState.payment.cardLastFour,
        CardBrand: checkoutState.payment.cardBrand,

        Items: cart.map(i => ({ ArtifactId: i.id, Quantity: i.quantity }))
    };

    const r = await post("/api/orders", dto);
    if (!r.ok) {
        toast(r.data?.message || "Failed to place order. Please try again.", "error");
        return;
    }

    // Success
    cart = [];
    saveCart();
    
    // Show success screen
    document.getElementById('checkout-flow').style.display = 'none';
    const successDiv = document.getElementById('checkout-success');
    successDiv.style.display = 'block';
    successDiv.innerHTML = `
        <div class="order-success">
            <div class="order-success-icon">✓</div>
            <h2>Order Confirmed!</h2>
            <p style="font-size:1.1rem; color:var(--earth-muted); margin-bottom:2rem;">Thank you, ${dto.BillingFirstName}. Your order #${r.data.id} has been placed successfully.</p>
            <p style="margin-bottom:2rem;">We've sent a confirmation email to your registered email address.</p>
            <button class="btn btn-primary" onclick="showPage('home')">Return to Home</button>
        </div>
    `;
}

// Initial setup
document.addEventListener('DOMContentLoaded', () => {
    // Set min date for delivery to today + 3 days
    const dateInput = document.getElementById('delivery-date');
    if (dateInput) {
        const d = new Date();
        d.setDate(d.getDate() + 3);
        dateInput.min = d.toISOString().split('T')[0];
    }
});
