using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Customerwallet
{
    public int WalletId { get; set; }

    public int CustomerId { get; set; }

    public decimal? Balance { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
