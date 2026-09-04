using System;

namespace Ecommerce.Models;

public partial class Sellerwarning
{
    public int WarningId { get; set; }

    public int SellerId { get; set; }

    public int? FeedbackId { get; set; }

    public string? WarningReason { get; set; }

    public DateTime? WarningDate { get; set; }

    public string? Status { get; set; }

    public virtual Customerfeedback? Feedback { get; set; }

    public virtual Seller Seller { get; set; } = null!;
}
