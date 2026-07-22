using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Pricedropnotification
{
    public int PriceDropId { get; set; }

    public int CustomerId { get; set; }

    public int ProductId { get; set; }

    public decimal? OldPrice { get; set; }

    public decimal? NewPrice { get; set; }

    public DateTime? NotificationDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
