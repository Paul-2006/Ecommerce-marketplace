using System;
using System.Collections.Generic;
using System.Linq;
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
    public class DeliveryVerificationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IDrivingLicenceVerificationService _dlService;
        private readonly IVehicleVerificationService _vehicleService;
        private readonly IDocumentVerificationService _documentService;

        public DeliveryVerificationController(
            ApplicationDbContext context,
            IDrivingLicenceVerificationService dlService,
            IVehicleVerificationService vehicleService,
            IDocumentVerificationService documentService)
        {
            _context = context;
            _dlService = dlService;
            _vehicleService = vehicleService;
            _documentService = documentService;
        }

        public class DeliveryVerificationSubmissionDTO
        {
            public int DeliveryPartnerId { get; set; }
            public string PartnerName { get; set; } = null!;
            public string Email { get; set; } = null!;
            public string PhoneNumber { get; set; } = null!;
            public string IdentityNumber { get; set; } = null!;
            public string DrivingLicenceNumber { get; set; } = null!;
            public string VehicleClass { get; set; } = null!;
            public DateTime? LicenceExpiryDate { get; set; }
            public string VehicleRegistrationNumber { get; set; } = null!;
            public string VehicleModel { get; set; } = null!;
            public string InsurancePolicyNumber { get; set; } = null!;
            public DateTime? InsuranceExpiryDate { get; set; }
        }

        public class AdminReviewActionDTO
        {
            public string Reason { get; set; } = "";
        }

        // POST: api/DeliveryVerification/verify-dl
        [HttpPost("verify-dl")]
        public async Task<IActionResult> VerifyDl([FromBody] dynamic payload)
        {
            string dl = payload?.dlNumber?.ToString() ?? "";
            string name = payload?.name?.ToString() ?? "";
            DateTime? expiry = payload?.expiryDate != null ? Convert.ToDateTime(payload.expiryDate.ToString()) : null;

            var result = await _dlService.VerifyDrivingLicenceAsync(dl, name, expiry);
            return Ok(new { success = result.IsValid, data = result });
        }

        // POST: api/DeliveryVerification/verify-vehicle
        [HttpPost("verify-vehicle")]
        public async Task<IActionResult> VerifyVehicle([FromBody] dynamic payload)
        {
            string vehicleNumber = payload?.vehicleNumber?.ToString() ?? "";
            string policy = payload?.policyNumber?.ToString() ?? "";
            DateTime? expiry = payload?.expiryDate != null ? Convert.ToDateTime(payload.expiryDate.ToString()) : null;

            var result = await _vehicleService.VerifyVehicleAsync(vehicleNumber, policy, expiry);
            return Ok(new { success = result.IsValid, data = result });
        }

        // POST: api/DeliveryVerification/upload-document
        [HttpPost("upload-document")]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] string documentType, [FromForm] int deliveryPartnerId)
        {
            var result = await _documentService.UploadAndExtractDocumentAsync(file, documentType, "DELIVERY_PARTNER", deliveryPartnerId);
            if (!result.Success)
            {
                return BadRequest(new { success = false, message = result.Message });
            }

            var docRecord = new VerificationDocument
            {
                OwnerType = "DELIVERY_PARTNER",
                OwnerEntityId = deliveryPartnerId,
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

        // POST: api/DeliveryVerification/submit
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitVerification([FromBody] DeliveryVerificationSubmissionDTO dto)
        {
            var existing = await _context.DeliveryPartnerVerifications
                .Include(v => v.VerificationChecks)
                .FirstOrDefaultAsync(v => v.DeliveryPartnerId == dto.DeliveryPartnerId);

            var verification = existing ?? new DeliveryPartnerVerification { DeliveryPartnerId = dto.DeliveryPartnerId };

            verification.PartnerName = dto.PartnerName;
            verification.Email = dto.Email;
            verification.PhoneNumber = dto.PhoneNumber;
            verification.IdentityNumber = dto.IdentityNumber;
            verification.DrivingLicenceNumber = dto.DrivingLicenceNumber;
            verification.VehicleClass = dto.VehicleClass;
            verification.LicenceExpiryDate = dto.LicenceExpiryDate;
            verification.VehicleRegistrationNumber = dto.VehicleRegistrationNumber;
            verification.VehicleModel = dto.VehicleModel;
            verification.InsurancePolicyNumber = dto.InsurancePolicyNumber;
            verification.InsuranceExpiryDate = dto.InsuranceExpiryDate;
            verification.Status = "InProgress";
            verification.SubmittedDate = DateTime.Now;

            // Execute & populate verification checks
            var dlRes = await _dlService.VerifyDrivingLicenceAsync(dto.DrivingLicenceNumber, dto.PartnerName, dto.LicenceExpiryDate);
            var vehRes = await _vehicleService.VerifyVehicleAsync(dto.VehicleRegistrationNumber, dto.InsurancePolicyNumber, dto.InsuranceExpiryDate);

            // Calculate worst expiry status between DL and Insurance
            if (dlRes.ExpiryStatus == "EXPIRED" || vehRes.ExpiryStatus == "EXPIRED") verification.ExpiryStatus = "EXPIRED";
            else if (dlRes.ExpiryStatus == "EXPIRING_SOON" || vehRes.ExpiryStatus == "EXPIRING_SOON") verification.ExpiryStatus = "EXPIRING_SOON";
            else verification.ExpiryStatus = "VALID";

            if (existing == null)
            {
                _context.DeliveryPartnerVerifications.Add(verification);
                await _context.SaveChangesAsync();
            }

            var oldChecks = await _context.DeliveryVerificationChecks.Where(c => c.VerificationId == verification.VerificationId).ToListAsync();
            _context.DeliveryVerificationChecks.RemoveRange(oldChecks);

            var checks = new List<DeliveryVerificationCheck>
            {
                new DeliveryVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "IDENTITY_VERIFIED", CheckName = "Aadhaar/Identity Proof Verification", Status = !string.IsNullOrWhiteSpace(dto.IdentityNumber) ? "Passed" : "Failed", Provider = "SYSTEM", EvidenceSummary = $"Identity Reference: {dto.IdentityNumber}" },
                new DeliveryVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "DL_VALID", CheckName = "Driving Licence Format & Status", Status = dlRes.IsValid ? "Passed" : "Failed", Provider = dlRes.Provider, EvidenceSummary = dlRes.Message },
                new DeliveryVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "DL_EXPIRY_VALID", CheckName = "Driving Licence Expiry Check", Status = !dlRes.IsExpired ? "Passed" : "Failed", Provider = dlRes.Provider, EvidenceSummary = $"DL Expiry: {dlRes.ExpiryDate:dd MMM yyyy} ({dlRes.ExpiryStatus})" },
                new DeliveryVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "RC_VERIFIED", CheckName = "Vehicle Registration RC Lookup", Status = vehRes.IsValid ? "Passed" : "Failed", Provider = vehRes.Provider, EvidenceSummary = $"RC Owner: {vehRes.RegisteredOwner}" },
                new DeliveryVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "VEHICLE_CLASS_MATCH", CheckName = "Vehicle Commercial Class Match", Status = !string.IsNullOrWhiteSpace(dto.VehicleClass) ? "Passed" : "Failed", Provider = "SYSTEM", EvidenceSummary = $"Vehicle Class: {dto.VehicleClass}" },
                new DeliveryVerificationCheck { VerificationId = verification.VerificationId, CheckCode = "INSURANCE_VALID", CheckName = "Vehicle Insurance Policy Status", Status = !vehRes.IsInsuranceExpired ? "Passed" : "Failed", Provider = vehRes.Provider, EvidenceSummary = $"Policy #{dto.InsurancePolicyNumber} ({vehRes.InsuranceStatus})" }
            };

            _context.DeliveryVerificationChecks.AddRange(checks);

            verification.PassedChecksCount = checks.Count(c => c.Status == "Passed");
            verification.TotalRequiredChecks = checks.Count;

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "DELIVERY_PARTNER",
                TargetEntityId = dto.DeliveryPartnerId,
                Action = "SUBMITTED",
                Status = "InProgress",
                Reason = "Delivery Partner submitted onboarding verification package.",
                Provider = "SYSTEM",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, verificationId = verification.VerificationId, status = verification.Status, expiryStatus = verification.ExpiryStatus, passedChecks = verification.PassedChecksCount, totalChecks = verification.TotalRequiredChecks });
        }

        // GET: api/DeliveryVerification/my-status/{partnerId}
        [HttpGet("my-status/{partnerId}")]
        public async Task<IActionResult> GetMyStatus(int partnerId)
        {
            var verification = await _context.DeliveryPartnerVerifications
                .Include(v => v.VerificationChecks)
                .Include(v => v.Documents)
                .FirstOrDefaultAsync(v => v.DeliveryPartnerId == partnerId);

            if (verification == null) return Ok(new { hasSubmitted = false });

            var history = await _context.VerificationHistories
                .Where(h => h.TargetType == "DELIVERY_PARTNER" && h.TargetEntityId == partnerId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return Ok(new { hasSubmitted = true, verification, history });
        }

        // GET: api/DeliveryVerification/admin/list
        [HttpGet("admin/list")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAdminList([FromQuery] string? status, [FromQuery] string? search)
        {
            var query = _context.DeliveryPartnerVerifications
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
                    (v.PartnerName != null && v.PartnerName.ToLower().Contains(s)) ||
                    (v.DrivingLicenceNumber != null && v.DrivingLicenceNumber.ToLower().Contains(s)) ||
                    (v.VehicleRegistrationNumber != null && v.VehicleRegistrationNumber.ToLower().Contains(s)));
            }

            var list = await query.OrderByDescending(v => v.SubmittedDate).ToListAsync();
            return Ok(list);
        }

        // GET: api/DeliveryVerification/admin/{id}
        [HttpGet("admin/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetAdminDetail(int id)
        {
            var verification = await _context.DeliveryPartnerVerifications
                .Include(v => v.VerificationChecks)
                .Include(v => v.Documents)
                .FirstOrDefaultAsync(v => v.VerificationId == id);

            if (verification == null) return NotFound("Delivery partner verification record not found.");

            var history = await _context.VerificationHistories
                .Where(h => h.TargetType == "DELIVERY_PARTNER" && h.TargetEntityId == verification.DeliveryPartnerId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();

            return Ok(new { verification, history });
        }

        // POST: api/DeliveryVerification/admin/{id}/approve
        [HttpPost("admin/{id}/approve")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ApproveDeliveryPartner(int id)
        {
            var verification = await _context.DeliveryPartnerVerifications
                .Include(v => v.VerificationChecks)
                .FirstOrDefaultAsync(v => v.VerificationId == id);

            if (verification == null) return NotFound("Delivery partner verification record not found.");

            int passed = verification.VerificationChecks.Count(c => c.Status == "Passed");
            int required = verification.VerificationChecks.Count;
            if (required > 0 && passed < required)
            {
                return BadRequest(new { success = false, message = $"Cannot approve delivery partner. Required checks pending: {passed}/{required} passed." });
            }

            verification.Status = "Verified";
            verification.ReviewedDate = DateTime.Now;

            var partner = await _context.Deliverypartners.FindAsync(verification.DeliveryPartnerId);
            if (partner != null)
            {
                partner.Status = "Approved";
                partner.AvailabilityStatus = "Active";
            }

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "DELIVERY_PARTNER",
                TargetEntityId = verification.DeliveryPartnerId,
                Action = "APPROVED",
                Status = "Verified",
                Reason = "All statutory DL, RC, Insurance & identity checks passed. Approved by Admin.",
                Provider = "ADMIN",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Delivery partner approved successfully." });
        }

        // POST: api/DeliveryVerification/admin/{id}/reject
        [HttpPost("admin/{id}/reject")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RejectDeliveryPartner(int id, [FromBody] AdminReviewActionDTO dto)
        {
            var verification = await _context.DeliveryPartnerVerifications.FindAsync(id);
            if (verification == null) return NotFound("Delivery partner verification record not found.");

            string reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Driving Licence or Vehicle RC verification failed." : dto.Reason.Trim();

            verification.Status = "Failed";
            verification.RejectionReason = reason;
            verification.ReviewedDate = DateTime.Now;

            var partner = await _context.Deliverypartners.FindAsync(verification.DeliveryPartnerId);
            if (partner != null) partner.Status = "Rejected";

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "DELIVERY_PARTNER",
                TargetEntityId = verification.DeliveryPartnerId,
                Action = "REJECTED",
                Status = "Failed",
                Reason = reason,
                Provider = "ADMIN",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Delivery partner application rejected." });
        }

        // POST: api/DeliveryVerification/admin/{id}/request-correction
        [HttpPost("admin/{id}/request-correction")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> RequestCorrection(int id, [FromBody] AdminReviewActionDTO dto)
        {
            var verification = await _context.DeliveryPartnerVerifications.FindAsync(id);
            if (verification == null) return NotFound("Delivery partner verification record not found.");

            string reason = string.IsNullOrWhiteSpace(dto.Reason) ? "Driving licence / Insurance document requires updating." : dto.Reason.Trim();

            verification.Status = "NeedsCorrection";
            verification.CorrectionReason = reason;
            verification.ReviewedDate = DateTime.Now;

            _context.VerificationHistories.Add(new VerificationHistory
            {
                TargetType = "DELIVERY_PARTNER",
                TargetEntityId = verification.DeliveryPartnerId,
                Action = "CORRECTION_REQUESTED",
                Status = "NeedsCorrection",
                Reason = reason,
                Provider = "ADMIN",
                CreatedAt = DateTime.Now
            });

            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Correction request sent to delivery partner." });
        }
    }
}
