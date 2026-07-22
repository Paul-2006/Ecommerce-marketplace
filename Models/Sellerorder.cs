using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Sellerorder
{
    public int SellerOrderId { get; set; }

    public int OrderId { get; set; }

    public int SellerId { get; set; }

    public string? SellerOrderStatus { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Seller Seller { get; set; } = null!;
}
