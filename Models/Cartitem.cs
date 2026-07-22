using System;
using Ecommerce.Models;

namespace Ecommerce.Models;

public partial class Cartitem
{
    public int CartItemId { get; set; }

    public int CartId { get; set; }

    public int SellerProductId { get; set; }

    public int? ProductId { get; set; }
    public int Quantity { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Sellerproduct SellerProduct { get; set; } = null!;
}