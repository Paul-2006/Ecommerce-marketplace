using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Productcomparison
{
    public int ComparisonId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public DateTime? AddedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
