namespace Ecommerce.DTOs
{

    public class OrderStatusDTO
    {

        public int OrderId { get; set; }


        public string Status { get; set; } = null!;


        public string? Remarks { get; set; }


    }

}