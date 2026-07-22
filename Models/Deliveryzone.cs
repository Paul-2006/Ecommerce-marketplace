using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliveryzone
{
    public int ZoneId { get; set; }

    public string? ZoneName { get; set; }

    public decimal? CenterLatitude { get; set; }

    public decimal? CenterLongitude { get; set; }

    public int? RadiusKm { get; set; }

    public virtual ICollection<Deliverypartnerzone> Deliverypartnerzones { get; set; } = new List<Deliverypartnerzone>();
}
