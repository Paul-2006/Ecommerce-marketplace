using System;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class AdminVerificationCenterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminVerificationCenterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/AdminVerificationCenter/summary
        [HttpGet("summary")]
        public async Task<IActionResult> GetSummaryMetrics()
        {
            // Sellers by status
            var sellerPending = await _context.SellerVerifications.CountAsync(s => s.Status == "Pending");
            var sellerInProgress = await _context.SellerVerifications.CountAsync(s => s.Status == "InProgress");
            var sellerVerified = await _context.SellerVerifications.CountAsync(s => s.Status == "Verified");
            var sellerNeedsCorrection = await _context.SellerVerifications.CountAsync(s => s.Status == "NeedsCorrection");
            var sellerFailed = await _context.SellerVerifications.CountAsync(s => s.Status == "Failed");

            // Delivery partners by status
            var deliveryPending = await _context.DeliveryPartnerVerifications.CountAsync(d => d.Status == "Pending");
            var deliveryInProgress = await _context.DeliveryPartnerVerifications.CountAsync(d => d.Status == "InProgress");
            var deliveryVerified = await _context.DeliveryPartnerVerifications.CountAsync(d => d.Status == "Verified");
            var deliveryNeedsCorrection = await _context.DeliveryPartnerVerifications.CountAsync(d => d.Status == "NeedsCorrection");
            var deliveryFailed = await _context.DeliveryPartnerVerifications.CountAsync(d => d.Status == "Failed");

            // Expiry alerts
            var expiringLicences = await _context.DeliveryPartnerVerifications.CountAsync(d => d.ExpiryStatus == "EXPIRING_SOON");
            var expiredLicences = await _context.DeliveryPartnerVerifications.CountAsync(d => d.ExpiryStatus == "EXPIRED");

            return Ok(new
            {
                sellers = new
                {
                    pending = sellerPending,
                    inProgress = sellerInProgress,
                    verified = sellerVerified,
                    needsCorrection = sellerNeedsCorrection,
                    failed = sellerFailed,
                    total = sellerPending + sellerInProgress + sellerVerified + sellerNeedsCorrection + sellerFailed
                },
                deliveryPartners = new
                {
                    pending = deliveryPending,
                    inProgress = deliveryInProgress,
                    verified = deliveryVerified,
                    needsCorrection = deliveryNeedsCorrection,
                    failed = deliveryFailed,
                    expiringSoon = expiringLicences,
                    expired = expiredLicences,
                    total = deliveryPending + deliveryInProgress + deliveryVerified + deliveryNeedsCorrection + deliveryFailed
                },
                serverTime = DateTime.Now
            });
        }
    }
}
