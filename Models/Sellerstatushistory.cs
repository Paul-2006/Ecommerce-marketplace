using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Sellerstatushistory
{
    public int HistoryId { get; set; }

    public int SellerId { get; set; }

    public string? PreviousStatus { get; set; }

    public string? NewStatus { get; set; }

    public string? Reason { get; set; }

    public DateTime? ChangedDate { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }
    public virtual Seller Seller { get; set; } = null!;
}
