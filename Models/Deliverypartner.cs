using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Deliverypartner
{
    public int DeliveryPartnerId { get; set; }

    public int UserId { get; set; }

    public string? VehicleNumber { get; set; }

    public decimal? CurrentLatitude { get; set; }

    public decimal? CurrentLongitude { get; set; }

    public string? AvailabilityStatus { get; set; }

    public string? PartnerName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Status { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DateTime? CreatedDate { get; set; }
    public virtual ICollection<Deliveryassignment> Deliveryassignments { get; set; } = new List<Deliveryassignment>();

    public virtual ICollection<Deliverylocationtracking> Deliverylocationtrackings { get; set; } = new List<Deliverylocationtracking>();

    public virtual ICollection<Deliverypartnerzone> Deliverypartnerzones { get; set; } = new List<Deliverypartnerzone>();

    public virtual ICollection<Faileddelivery> Faileddeliveries { get; set; } = new List<Faileddelivery>();

    public virtual User User { get; set; } = null!;
}
