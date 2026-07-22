namespace Ecommerce.DTOs;

public class LocationTrackingDTO
{
    public int DeliveryPartnerId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}