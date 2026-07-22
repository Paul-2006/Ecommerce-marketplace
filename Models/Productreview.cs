using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Productreview
{
    public int ReviewId { get; set; }

    public int ProductId { get; set; }

    public int CustomerId { get; set; }

    public int? Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
