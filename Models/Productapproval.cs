using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Productapproval
{
    public int ApprovalId { get; set; }

    public int ProductId { get; set; }

    public int AdminId { get; set; }

    public string? ApprovalStatus { get; set; }

    public string? Remarks { get; set; }

    public DateTime? ApprovalDate { get; set; }

    public virtual User Admin { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
