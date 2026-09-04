using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Productimage
{
    public int ImageId { get; set; }

    public int ProductId { get; set; }

    public string? ImageUrl { get; set; }

    public bool? IsPrimary { get; set; }

    public DateTime? UploadedDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
