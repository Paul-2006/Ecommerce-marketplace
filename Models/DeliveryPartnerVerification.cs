using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class DeliveryPartnerVerification
{
    public int VerificationId { get; set; }

    public int DeliveryPartnerId { get; set; }

    public string? PartnerName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? IdentityNumber { get; set; } // Aadhaar/Voter/Passport

    public string? DrivingLicenceNumber { get; set; }

    public string? VehicleClass { get; set; } // LMV, Two-Wheeler, Commercial

    public DateTime? LicenceExpiryDate { get; set; }

    public string? VehicleRegistrationNumber { get; set; }

    public string? VehicleModel { get; set; }

    public string? InsurancePolicyNumber { get; set; }

    public DateTime? InsuranceExpiryDate { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, InProgress, Verified, Failed, NeedsCorrection, ManualReview

    public string ExpiryStatus { get; set; } = "VALID"; // VALID, EXPIRING_SOON, EXPIRED

    public int PassedChecksCount { get; set; } = 0;

    public int TotalRequiredChecks { get; set; } = 6;

    public string? CorrectionReason { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime SubmittedDate { get; set; } = DateTime.Now;

    public DateTime? ReviewedDate { get; set; }

    public int? ReviewedByAdminId { get; set; }

    public virtual Deliverypartner DeliveryPartner { get; set; } = null!;

    public virtual ICollection<DeliveryVerificationCheck> VerificationChecks { get; set; } = new List<DeliveryVerificationCheck>();

    public virtual ICollection<VerificationDocument> Documents { get; set; } = new List<VerificationDocument>();
}
