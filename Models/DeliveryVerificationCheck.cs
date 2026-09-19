using System;

namespace Ecommerce.Models;

public partial class DeliveryVerificationCheck
{
    public int CheckId { get; set; }

    public int VerificationId { get; set; }

    public string CheckCode { get; set; } = null!; // IDENTITY_VERIFIED, DL_VALID, DL_EXPIRY_VALID, RC_VERIFIED, VEHICLE_CLASS_MATCH, INSURANCE_VALID, DOCUMENT_VERIFIED

    public string CheckName { get; set; } = null!;

    public string Status { get; set; } = "Pending"; // Passed, Failed, Pending, NeedsReview

    public string Provider { get; set; } = "MOCK"; // API_SETU, VAHAN, MOCK

    public decimal MatchConfidence { get; set; } = 1.0m;

    public string? EvidenceSummary { get; set; }

    public string? RawResultJson { get; set; }

    public DateTime CheckedAt { get; set; } = DateTime.Now;

    public virtual DeliveryPartnerVerification Verification { get; set; } = null!;
}
