namespace Ecommerce.DTOs
{

    public class DeliveryAssignmentDTO
    {

        public int OrderId { get; set; }


        public int DeliveryPartnerId { get; set; }


        public string Status { get; set; } = "Assigned";


    }

}