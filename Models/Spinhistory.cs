using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Spinhistory
{
    public int SpinId { get; set; }

    public int CustomerId { get; set; }

    public int RewardId { get; set; }

    public DateTime? SpinDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Spinreward Reward { get; set; } = null!;
}
