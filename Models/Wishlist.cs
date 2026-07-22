using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Wishlist
{
    public int WishlistId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Wishlistitem> Wishlistitems { get; set; } = new List<Wishlistitem>();
}