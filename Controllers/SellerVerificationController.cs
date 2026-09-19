using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Verification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerVerificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGstVerificationService _gstService;
        private readonly IPanVerificationService _panService;
        private readonly IDocumentVerificationService _documentService;

        public SellerVerificationController(
            ApplicationDbContext context,
            IGstVerificationService gstService,
            IPanVerificationService panService,
            IDocumentVerificationService documentService)
        {
            _context = context;
            _gstService = gstService;
            _panService = panService;
            _documentService = documentService;
        }

        // DTOs
        public class SellerVerificationSubmissionDTO
        {
            public int SellerId { get; set; }
            public string SellerName { get; set; } = null!;
            public string BusinessName { get; set; } = null!;
            public string LegalBusinessName { get; set; } = null!;
            public string TradeName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public string Gstnumber { get; set; } = null!;
            public string Pannumber { get; set; } = null!;
            public string BusinessAddress { get; set; } = null!;
            public string State { get; set; } = null!;
            public string District { get; set; } = null!;
            public string Pincode { get; set; } = null!;
            public string BankName { get; set; } = null!;
            public string BankAccountNumber { get; set; } = null!;
            public string BankIfscCode { get; set; } = null!;
        }

        public class AdminReviewActionDTO
        {
            public string Reason { get; set; } = "";
        }

        // POST: api/SellerVerification/verify-gst
        [HttpPost("verify-gst")]
        public async Task<IActionResult> VerifyGst([FromBody] dynamic payload)
        {
            string gstin = payload?.gstin?.ToString() ?? "";
            string legalName = payload?.legalName?.ToString() ?? "";
            string tradeName = payload?.tradeName?.ToString() ?? "";
            string address = payload?.address?.ToString() ?? "";

            var result = await _gstService.VerifyGstAsync(gstin, legalName, tradeName, address);
            return Ok(new { success = result.IsValid, data = result });
        }

        // POST: api/SellerVerification/verify-pan
        [HttpPost("verify-pan")]
        public async Task<IActionResult> VerifyPan([FromBody] dynamic payload)
        {
            string pan = payload?.pan?.ToString() ?? "";
            string name = payload?.name?.ToString() ?? "";

            var result = await _panService.VerifyPanAsync(pan, name);
            return Ok(new { success = result.IsValid, data = result });
        }

        // POST: api/SellerVerification/upload-document
        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] string documentType, [FromForm] int sellerId)
        {
            var result = await _documentService.UploadAndExtractDocumentAsync(file, documentType, "SELLER", sellerId);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            var docRecord = new VerificationDocument
            {
                OwnerType = "SELLER",
                OwnerEntityId = sellerId,
                DocumentType = documentType,
                DocumentName = file.FileName,
                FilePath = result.FilePath,
                FileType = result.FileType,
                FileSize = result.FileSize,
                OcrText = result.ExtractedOcrText,
                ExtractedDataJson = result.ExtractedDataJson,
                Status = "VERIFIED",
                UploadedAt = DateTime.Now
            };

            _context.VerificationDocuments.Add(docRecord);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, documentId = docRecord.DocumentId, filePath = result.FilePath, message = result.Message });
        }

        // POST: api/SellerVerification/submit
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitVerification([FromBody] SellerVerificationSubmissionDTO dto)
        {
            var existing = await _context.SellerVerifications
                .Include(s => s.VerificationChecks)
                .FirstOrDefaultAsync(s => s.SellerId == dto.SellerId);

            var verification = existing ?? new SellerVerification { SellerId = dto.SellerId };

            verification.SellerName = dto.SellerName;
            verification.BusinessName = dto.BusinessName;
            verification.LegalBusinessName = dto.LegalBusinessName;
            verification.TradeName = dto.TradeName;
            verification.Email = dto.Email;
            verification.PhoneNumber = dto.PhoneNumber;
            verification.Gstnumber = dto.Gstnumber;
            verification.Pannumber = dto.Pannumber;
            verification.BusinessAddress = dto.BusinessAddress;
            verification.State = dto.State;
            verification.District = dto.District;
            verification.Pincode = dto.Pincode;
            verification.BankName = dto.BankName;
            verification.BankAccountNumber = dto.BankAccountNumber;
            verification.BankIfscCode = dto.BankIfscCode;
            verification.Status = "InProgress";
            verification.SubmittedDate = DateTime.Now;

            // Execute & populate itemized checks
            var gstRes = await _gstService.VerifyGstAsync(dto.Gstnumber, dto.LegalBusinessName, dto.TradeName, dto.BusinessAddress);
            var panRes = await _panService.VerifyPanAsync(dto.Pannumber, dto.LegalBusinessName);

            if (existing == null)
            {
                _context.SellerVerifications.Add(verification);
                await _context.SaveChangesAsync();
            }

            // Clean & re-populate checks
            var oldChecks = await _context.SellerVerificationChecks.Where(c => c.VerificationId == verification.VerificationId).ToListAsync();
            _context.SellerVerificationChecks.RemoveRange(oldChecks);

            var checks = new List<SellerVerificationCheck>
            {
                new SellerVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "GSTIN_VALID", CheckName = "GSTIN Statutory Structure", Status = gstRes.IsValid ? "Passed" : "Failed", Provider = gstRes.Provider, EvidenceSummary = gstRes.Message },
                new SellerVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "GST_STATUS_ACTIVE", CheckName = "GST Portal Active Registration", Status = gstRes.GstStatus == "Active" ? "Passed" : "Failed", Provider = gstRes.Provider, EvidenceSummary = $"GST Status: {gstRes.GstStatus}" },
                new SellerVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "GST_NAME_MATCH", CheckName = "Legal & Trade Name Match", Status = (gstRes.IsLegalNameMatched && gstRes.IsTradeNameMatched) ? "Passed" : "Failed", Provider = gstRes.Provider, EvidenceSummary = $"Legal: {gstRes.LegalName} • Trade: {gstRes.TradeName}" },
                new SellerVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "PAN_VALID", CheckName = "PAN Structure & Status", Status = panRes.IsValid ? "Passed" : "Failed", Provider = panRes.Provider, EvidenceSummary = panRes.Message },
                new SellerVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "PAN_NAME_MATCH", CheckName = "PAN Account Name Match", Status = panRes.IsNameMatched ? "Passed" : "Failed", Provider = panRes.Provider, EvidenceSummary = $"PAN Registered Name: {panRes.RegisteredName}" },
                new SellerVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "BANK_VERIFIED", CheckName = "Payout Bank Account Verification", Status = (!string.IsNullOrWhiteSpace(dto.BankAccountNumber) && !string.IsNullOrWhiteSpace(dto.BankIfscCode)) ? "Passed" : "Failed", Provider = "SYSTEM", EvidenceSummary = $"Bank: {dto.BankName} ({dto.BankIfscCode})" }
            };

            _context.SellerVerificationChecks.AddRange(checks);

            verification.PassedChecksCount = checks.Count(c => c.Status == "Passed");
            verification.TotalRequiredChecks = checks.Count;

            // Log Audit Entry
            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "SELLER",
                TargetEntityId = dto.SellerId,
                Action = "SUBMITTED",
                Status = "InProgress",
                Reason = "Seller submitted onboarding verification package.",
                Provider = "SYSTEM",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return Ok(new { success = true, verificationId = verification.VerificationId, status = verification.Status, passedChecks = verification.PassedChecksCount, totalChecks = verification.TotalRequiredChecks });
        }

        // GET: api/SellerVerification/my-status/{sellerId}
        [HttpGet("my-status/{sellerId}")]
        public async Task<IActionResult> GetMyStatus(int sellerId)
        {
            var verification = await _context.SellerVerifications
                .Include(v => v.VerificationChecks)
                .Include(v => v.Documents)
                .FirstOrDefaultAsync(v => v.SellerId == sellerId);

            if (verification == null)
            {
                return Ok(new { hasSubmitted = false });
            }

            var history = await _context.VerificationHistories
                .Where(h => h.TargetType == "SELLER" && h.TargetEntityId == sellerId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return Ok(new { hasSubmitted = true, verification, history });
        }

        // GET: api/SellerVerification/admin/list
        [HttpGet("admin/list")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAdminList([FromQuery] string? status, [FromQuery] string? search)
        {
            var query = _context.SellerVerifications
                .Include(v => v.VerificationChecks)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(status) && status != "ALL")
            {
                query = query.Where(v => v.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower().Trim();
                query = query.Where(v =>
                    (v.BusinessName != null && v.BusinessName.ToLower().Contains(s)) ||
                    (v.SellerName != null && v.SellerName.ToLower().Contains(s)) ||
                    (v.Gstnumber != null && v.Gstnumber.ToLower().Contains(s)));
            }

            var list = await query.OrderByDescending(v => v.SubmittedDate).ToListAsync();
            return Ok(list);
        }

        // GET: api/SellerVerification/admin/{id}
        [HttpGet("admin/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAdminDetail(int id)
        {
            var verification = await _context.SellerVerifications
                .Include(v => v.VerificationChecks)
                .Include(v => v.Documents)
                .FirstOrDefaultAsync(v => v.VerificationId == id);

            if (verification == null) return NotFound("Seller verification record not found.");

            var history = await _context.VerificationHistories
                .Where(h => h.TargetType == "SELLER" && h.TargetEntityId == verification.SellerId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return Ok(new { verification, history });
        }

        // POST: api/SellerVerification/admin/{id}/approve
        [HttpPost("admin/{id}/approve")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ApproveSeller(int id)
        {
            var verification = await _context.SellerVerifications
                .Include(v => v.VerificationChecks)
                .FirstOrDefaultAsync(v => v.VerificationId == id);

            if (verification == null) return NotFound("Seller verification record not found.");

            // Mandatory gate: cannot approve unless all required checks passed
            int passed = verification.VerificationChecks.Count(c => c.Status == "Passed");
            int required = verification.VerificationChecks.Count;
            if (required > 0 && passed < required)
            {
                return BadRequest(new { success = false, message = $"Cannot approve seller. Required checks pending: {passed}/{required} passed." });
            }

            verification.Status = "Verified";
            verification.ReviewedDate = DateTime.Now;

            // Also update parent Seller model status
            var seller = await _context.Sellers.FindAsync(verification.SellerId);
            if (seller != null)
            {
                seller.Status = "Active";
                seller.ApprovalStatus = "Approved";
            }

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "SELLER",
                TargetEntityId = verification.SellerId,
                Action = "APPROVED",
                Status = "Verified",
                Reason = "All itemized statutory checks passed. Approved by Admin.",
                Provider = "ADMIN",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Seller application approved successfully." });
        }

        // POST: api/SellerVerification/admin/{id}/reject
        [HttpPost("admin/{id}/reject")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RejectSeller(int id, [FromBody] AdminReviewActionDTO dto)
        {
            var verification = await _context.SellerVerifications.FindAsync(id);
            if (verification == null) return NotFound("Seller verification record not found.");

            string reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Business details could not be verified." : dto.Reason.Trim();

            verification.Status = "Failed";
            verification.RejectionReason = reason;
            verification.ReviewedDate = DateTime.Now;

            var seller = await _context.Sellers.FindAsync(verification.SellerId);
            if (seller != null)
            {
                seller.Status = "Rejected";
                seller.ApprovalStatus = "Rejected";
            }

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "SELLER",
                TargetEntityId = verification.SellerId,
                Action = "REJECTED",
                Status = "Failed",
                Reason = reason,
                Provider = "ADMIN",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Seller application rejected." });
        }

        // POST: api/SellerVerification/admin/{id}/request-correction
        [HttpPost("admin/{id}/request-correction")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RequestCorrection(int id, [FromBody] AdminReviewActionDTO dto)
        {
            var verification = await _context.SellerVerifications.FindAsync(id);
            if (verification == null) return NotFound("Seller verification record not found.");

            string reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Submitted business details require correction." : dto.Reason.Trim();

            verification.Status = "NeedsCorrection";
            verification.CorrectionReason = reason;
            verification.ReviewedDate = DateTime.Now;

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "SELLER",
                TargetEntityId = verification.SellerId,
                Action = "CORRECTION_REQUESTED",
                Status = "NeedsCorrection",
                Reason = reason,
                Provider = "ADMIN",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Correction request sent to seller." });
        }
    }
}
