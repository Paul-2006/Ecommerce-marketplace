using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Seller
{
    public int SellerId { get; set; }

    public int UserId { get; set; }

    public string? BusinessName { get; set; }

    public string? BusinessAddress { get; set; }

    public string? Gstnumber { get; set; }

    public string? ApprovalStatus { get; set; }

    public int? ComplaintCount { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Status { get; set; }



    public virtual ICollection<Customerfeedback> Customerfeedbacks { get; set; } = new List<Customerfeedback>();

    public virtual ICollection<Sellerorder> Sellerorders { get; set; } = new List<Sellerorder>();

    public virtual ICollection<Sellerperformance> Sellerperformances { get; set; } = new List<Sellerperformance>();

    public virtual ICollection<Sellerproduct> Sellerproducts { get; set; } = new List<Sellerproduct>();

    public virtual ICollection<Sellerstatushistory> Sellerstatushistories { get; set; } = new List<Sellerstatushistory>();

    public virtual ICollection<Sellerwarning> Sellerwarnings { get; set; } = new List<Sellerwarning>();

    public virtual User User { get; set; } = null!;
}
