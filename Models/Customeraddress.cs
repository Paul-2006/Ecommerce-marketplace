using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Customeraddress
{
    public int AddressId { get; set; }

    public int CustomerId { get; set; }

    public string? AddressLine { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Pincode { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public string? AddressType { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool? IsDefault { get; set; }
    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
