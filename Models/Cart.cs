using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Cartitem> Cartitems { get; set; }
        = new List<Cartitem>();
}