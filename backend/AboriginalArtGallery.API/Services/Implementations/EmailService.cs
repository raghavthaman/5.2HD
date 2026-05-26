using System.Net;
using System.Net.Mail;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    private bool IsConfigured =>
        !string.IsNullOrEmpty(_config["SmtpSettings:Host"]) &&
        !string.IsNullOrEmpty(_config["SmtpSettings:Username"]);

    public async Task SendOtpEmailAsync(string toEmail, string otp)
    {
        var subject = "Your Yilpinji Verification Code";
        var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background:#f5f0eb; margin:0; padding:40px;'>
  <div style='max-width:480px; margin:0 auto; background:#fff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);'>
    <div style='background:#2d1f14; padding:32px; text-align:center;'>
      <h1 style='color:#c97b3a; font-size:1.8rem; margin:0;'>Yilpinji<span style=""color:#f0e0cc"">.</span></h1>
      <p style='color:#a89880; margin:8px 0 0; font-size:0.9rem;'>Aboriginal Art Gallery</p>
    </div>
    <div style='padding:40px; text-align:center;'>
      <h2 style='color:#2d1f14; margin:0 0 16px;'>Verify your email</h2>
      <p style='color:#6a5040; margin:0 0 32px;'>Use the code below to complete your registration. It expires in <strong>10 minutes</strong>.</p>
      <div style='background:#f5f0eb; border-radius:12px; padding:28px; margin:0 0 32px;'>
        <span style='font-size:2.5rem; font-weight:800; letter-spacing:12px; color:#c97b3a;'>{otp}</span>
      </div>
      <p style='color:#a89880; font-size:0.8rem;'>If you didn't request this, you can safely ignore this email.</p>
    </div>
  </div>
</body>
</html>";

        await SendAsync(toEmail, subject, body);
    }

    public async Task SendOrderConfirmationAsync(string toEmail, string username, Order order)
    {
        var itemRows = string.Join("", order.OrderItems.Select(i => $@"
          <tr>
            <td style='padding:10px; border-bottom:1px solid #f0e8df;'>{i.ArtifactTitle}</td>
            <td style='padding:10px; border-bottom:1px solid #f0e8df; text-align:center;'>{i.Quantity}</td>
            <td style='padding:10px; border-bottom:1px solid #f0e8df; text-align:right;'>${i.LineTotal:F2}</td>
          </tr>"));

        var subject = $"Order #{order.Id} Confirmed — Yilpinji Gallery";
        var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; background:#f5f0eb; margin:0; padding:40px;'>
  <div style='max-width:560px; margin:0 auto; background:#fff; border-radius:12px; overflow:hidden; box-shadow:0 4px 20px rgba(0,0,0,0.08);'>
    <div style='background:#2d1f14; padding:32px; text-align:center;'>
      <h1 style='color:#c97b3a; font-size:1.8rem; margin:0;'>Yilpinji<span style=""color:#f0e0cc"">.</span></h1>
      <p style='color:#a89880; margin:8px 0 0; font-size:0.9rem;'>Order Confirmation</p>
    </div>
    <div style='padding:40px;'>
      <h2 style='color:#2d1f14; margin:0 0 8px;'>Thank you, {username}! 🎨</h2>
      <p style='color:#6a5040; margin:0 0 24px;'>Your order <strong>#{order.Id}</strong> has been placed successfully and is being processed.</p>

      <table style='width:100%; border-collapse:collapse; margin-bottom:24px;'>
        <thead>
          <tr style='background:#f5f0eb;'>
            <th style='padding:10px; text-align:left; color:#8a7060; font-size:0.8rem; text-transform:uppercase;'>Item</th>
            <th style='padding:10px; text-align:center; color:#8a7060; font-size:0.8rem; text-transform:uppercase;'>Qty</th>
            <th style='padding:10px; text-align:right; color:#8a7060; font-size:0.8rem; text-transform:uppercase;'>Price</th>
          </tr>
        </thead>
        <tbody>{itemRows}</tbody>
      </table>

      <div style='background:#f5f0eb; border-radius:8px; padding:20px;'>
        <div style='display:flex; justify-content:space-between; margin-bottom:8px; color:#6a5040;'>
          <span>Subtotal</span><span>${order.SubTotal:F2}</span>
        </div>
        {(order.DiscountAmount > 0 ? $"<div style='display:flex; justify-content:space-between; margin-bottom:8px; color:#5a8a5a; font-weight:600;'><span>Discount</span><span>-${order.DiscountAmount:F2}</span></div>" : "")}
        <div style='display:flex; justify-content:space-between; margin-bottom:8px; color:#6a5040;'>
          <span>Shipping</span><span>{(order.ShippingCost == 0 ? "FREE" : $"${order.ShippingCost:F2}")}</span>
        </div>
        <div style='display:flex; justify-content:space-between; margin-bottom:8px; color:#6a5040;'>
          <span>GST (10%)</span><span>${order.TaxAmount:F2}</span>
        </div>
        <div style='display:flex; justify-content:space-between; margin-top:16px; padding-top:16px; border-top:2px solid #d4c4b0; font-weight:700; font-size:1.1rem; color:#2d1f14;'>
          <span>Total</span><span>${order.TotalAmount:F2} AUD</span>
        </div>
      </div>

      <div style='margin-top:24px; padding:16px; border-left:3px solid #c97b3a; background:#fdf8f3;'>
        <p style='margin:0; color:#6a5040; font-size:0.9rem;'>
          <strong>Delivery to:</strong> {order.DeliveryAddressLine1}, {order.DeliveryCity} {order.DeliveryPostCode}<br>
          <strong>Preferred date:</strong> {order.PreferredDeliveryDate:dd MMM yyyy} ({order.DeliveryTimeSlot})
        </p>
      </div>

      <p style='margin-top:32px; color:#a89880; font-size:0.8rem; text-align:center;'>
        © 2026 Yilpinji Aboriginal Art Gallery. We acknowledge the Traditional Custodians of the land.
      </p>
    </div>
  </div>
</body>
</html>";

        await SendAsync(toEmail, subject, body);
    }

    private async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (!IsConfigured)
        {
            _logger.LogInformation(
                "📧 [EMAIL - NO SMTP CONFIGURED]\nTo: {Email}\nSubject: {Subject}\nBody preview: {Preview}",
                toEmail, subject, htmlBody.Length > 200 ? htmlBody[..200] + "..." : htmlBody);
            return;
        }

        try
        {
            var host = _config["SmtpSettings:Host"]!;
            var port = int.Parse(_config["SmtpSettings:Port"] ?? "587");
            var username = _config["SmtpSettings:Username"]!;
            var password = _config["SmtpSettings:Password"]!;
            var fromEmail = _config["SmtpSettings:FromEmail"] ?? username;
            var fromName = _config["SmtpSettings:FromName"] ?? "Yilpinji Gallery";

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("📧 Email sent to {Email}: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send email to {Email}", toEmail);
        }
    }
}
