using System;

namespace Ecommerce.Models;

public partial class Orderitem
{
    public int OrderItemId { get; set; }


    public int OrderId { get; set; }


    public int SellerProductId { get; set; }


    public int? Quantity { get; set; }


    public decimal? Price { get; set; }



    public virtual Order Order { get; set; } = null!;


    public virtual Sellerproduct SellerProduct { get; set; } = null!;
}