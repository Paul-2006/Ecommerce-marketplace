using System;

namespace Ecommerce.Models;

public partial class SellerVerificationCheck
{
    public int CheckId { get; set; }

    public int VerificationId { get; set; }

    public string CheckCode { get; set; } = null!; // GSTIN_VALID, GST_STATUS_ACTIVE, GST_NAME_MATCH, PAN_VALID, PAN_NAME_MATCH, BUSINESS_DETAILS_MATCH, DOCUMENT_VERIFIED, BANK_VERIFIED

    public string CheckName { get; set; } = null!;

    public string Status { get; set; } = "Pending"; // Passed, Failed, Pending, NeedsReview

    public string Provider { get; set; } = "MOCK"; // API_SETU, GST_PORTAL, MOCK

    public decimal MatchConfidence { get; set; } = 1.0m;

    public string? EvidenceSummary { get; set; }

    public string? RawResultJson { get; set; }

    public DateTime CheckedAt { get; set; } = DateTime.Now;

    public virtual SellerVerification Verification { get; set; } = null!;
}
