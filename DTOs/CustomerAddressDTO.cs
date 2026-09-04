namespace Ecommerce.DTOs
{
    public class CustomerAddressDTO
    {

        public int CustomerId { get; set; }


        public string FullName { get; set; } = null!;


        public string PhoneNumber { get; set; } = null!;


        public string AddressLine { get; set; } = null!;


        public string City { get; set; } = null!;


        public string State { get; set; } = null!;


        public string Pincode { get; set; } = null!;


        public bool IsDefault { get; set; }


        public decimal? Latitude { get; set; }


        public decimal? Longitude { get; set; }

    }
}
