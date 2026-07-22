using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Sellerperformance
{
    public int PerformanceId { get; set; }

    public int SellerId { get; set; }

    public int? TotalOrders { get; set; }

    public int? CompletedOrders { get; set; }

    public int? FailedDeliveries { get; set; }

    public decimal? AverageRating { get; set; }

    public int? TotalComplaints { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Seller Seller { get; set; } = null!;
}
