using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Sellerwarning
{
    public int WarningId { get; set; }

    public int SellerId { get; set; }

    public int FeedbackId { get; set; }

    public string? WarningReason { get; set; }

    public string? WarningLevel { get; set; }

    public DateTime? IssuedDate { get; set; }

    public virtual Customerfeedback Feedback { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;
}
