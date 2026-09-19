using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class SellerVerification
{
    public int VerificationId { get; set; }

    public int SellerId { get; set; }

    public string? SellerName { get; set; }

    public string? BusinessName { get; set; }

    public string? LegalBusinessName { get; set; }

    public string? TradeName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Gstnumber { get; set; }

    public string? Pannumber { get; set; }

    public string? BusinessAddress { get; set; }

    public string? State { get; set; }

    public string? District { get; set; }

    public string? Pincode { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankIfscCode { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, InProgress, Verified, Failed, NeedsCorrection, ManualReview

    public int PassedChecksCount { get; set; } = 0;

    public int TotalRequiredChecks { get; set; } = 6;

    public string? CorrectionReason { get; set; }

    public string? RejectionReason { get; set; }

    public DateTime SubmittedDate { get; set; } = DateTime.Now;

    public DateTime? ReviewedDate { get; set; }

    public int? ReviewedByAdminId { get; set; }

    public virtual Seller Seller { get; set; } = null!;

    public virtual ICollection<SellerVerificationCheck> VerificationChecks { get; set; } = new List<SellerVerificationCheck>();

    public virtual ICollection<VerificationDocument> Documents { get; set; } = new List<VerificationDocument>();
}
