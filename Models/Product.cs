using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Product
{

    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Brand { get; set; }

    public string? Warranty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ApprovalStatus { get; set; }



    // Category
    public virtual Category Category { get; set; } = null!;



    // Product approvals
    public virtual ICollection<Productapproval> Productapprovals { get; set; }
        = new List<Productapproval>();



    // Seller products
    public virtual ICollection<Sellerproduct> Sellerproducts { get; set; }
        = new List<Sellerproduct>();



    // Cart items
    public virtual ICollection<Cartitem> Cartitems { get; set; }
        = new List<Cartitem>();



    // Wishlist items
    public virtual ICollection<Wishlistitem> Wishlistitems { get; set; }
        = new List<Wishlistitem>();



    // Price drop notifications
    public virtual ICollection<Pricedropnotification> Pricedropnotifications { get; set; }
        = new List<Pricedropnotification>();



    // Product reviews
    public virtual ICollection<Productreview> Productreviews { get; set; }
        = new List<Productreview>();



    // Product images
    public virtual ICollection<Productimage> Productimages { get; set; }
        = new List<Productimage>();



    // Product specifications
    public virtual ICollection<Productspecification> Productspecifications { get; set; }
        = new List<Productspecification>();



    // Product recommendations
    public virtual ICollection<Productrecommendation> Productrecommendations { get; set; }
        = new List<Productrecommendation>();



    // Product comparisons
    public virtual ICollection<Productcomparison> Productcomparisons { get; set; }
        = new List<Productcomparison>();

}
