using System;

namespace Ecommerce.Models;

public partial class VerificationHistory
{
    public int HistoryId { get; set; }

    public string TargetType { get; set; } = "SELLER"; // SELLER, DELIVERY_PARTNER

    public int TargetEntityId { get; set; }

    public string Action { get; set; } = null!; // SUBMITTED, GST_CHECK_PASSED, DL_CHECK_PASSED, APPROVED, REJECTED, CORRECTION_REQUESTED

    public string Status { get; set; } = null!; // Pending, InProgress, Verified, Failed, NeedsCorrection, ManualReview

    public string? Reason { get; set; }

    public string Provider { get; set; } = "SYSTEM"; // SYSTEM, GST_PORTAL, VAHAN, MOCK, ADMIN

    public int? AdminId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
