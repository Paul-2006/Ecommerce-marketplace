using System;

namespace Ecommerce.Models;

public partial class Wishlistitem
{
    public int WishlistItemId { get; set; }

    public int WishlistId { get; set; }

    public int ProductId { get; set; }

    public virtual Wishlist Wishlist { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
} 