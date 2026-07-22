using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Customerfeedback
{
    public int FeedbackId { get; set; }

    public int CustomerId { get; set; }

    public int? OrderId { get; set; }

    public int? SellerId { get; set; }

    public string? FeedbackType { get; set; }

    public int? Rating { get; set; }

    public string? ComplaintDescription { get; set; }

    public string? FeedbackStatus { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order? Order { get; set; }

    public virtual Seller? Seller { get; set; }

    public virtual ICollection<Sellerwarning> Sellerwarnings { get; set; } = new List<Sellerwarning>();
}
