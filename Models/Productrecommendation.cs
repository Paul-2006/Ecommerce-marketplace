using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Productrecommendation
{
    public int RecommendationId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public string? RecommendationReason { get; set; }

    public decimal? Score { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int ProductRecommendationId { get; set; }

    public string? Reason { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
