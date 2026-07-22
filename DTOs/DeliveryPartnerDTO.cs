namespace Ecommerce.DTOs
{

    public class DeliveryPartnerDTO
    {

        public string PartnerName { get; set; } = null!;


        public string PhoneNumber { get; set; } = null!;


        public string VehicleNumber { get; set; } = null!;


        public string Status { get; set; } = "Available";


        public double Latitude { get; set; }


        public double Longitude { get; set; }


    }

}