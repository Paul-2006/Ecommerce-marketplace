using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Spinreward
{
    public int RewardId { get; set; }

    public string? RewardName { get; set; }

    public int? DiscountPercentage { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Spinhistory> Spinhistories { get; set; } = new List<Spinhistory>();
}
