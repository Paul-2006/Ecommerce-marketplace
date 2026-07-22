using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Sellerproduct
{

    public int SellerProductId { get; set; }


    public int SellerId { get; set; }


    public int ProductId { get; set; }


    public decimal Price { get; set; }


    public decimal? Discount { get; set; }


    public int? StockQuantity { get; set; }


    public string ProductStatus { get; set; } = null!;



    public virtual Product Product { get; set; } = null!;


    public virtual Seller Seller { get; set; } = null!;


    public virtual ICollection<Cartitem> Cartitems { get; set; }
        = new List<Cartitem>();


    public virtual ICollection<Orderitem> Orderitems { get; set; }
        = new List<Orderitem>();
}