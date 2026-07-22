using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public int UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Customeraddress> Customeraddresses { get; set; } = new List<Customeraddress>();

    public virtual ICollection<Customerfeedback> Customerfeedbacks { get; set; } = new List<Customerfeedback>();

    public virtual ICollection<Customerwallet> Customerwallets { get; set; } = new List<Customerwallet>();

    public virtual ICollection<Deliveryinstruction> Deliveryinstructions { get; set; } = new List<Deliveryinstruction>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Pricedropnotification> Pricedropnotifications { get; set; } = new List<Pricedropnotification>();

    public virtual ICollection<Productcomparison> Productcomparisons { get; set; } = new List<Productcomparison>();

    public virtual ICollection<Productrecommendation> Productrecommendations { get; set; } = new List<Productrecommendation>();

    public virtual ICollection<Productreview> Productreviews { get; set; } = new List<Productreview>();

    public virtual ICollection<Searchhistory> Searchhistories { get; set; } = new List<Searchhistory>();

    public virtual ICollection<Spinhistory> Spinhistories { get; set; } = new List<Spinhistory>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
