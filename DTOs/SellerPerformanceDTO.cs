namespace Ecommerce.DTOs
{

    public class SellerPerformanceDTO
    {

        public int SellerId { get; set; }


        public int TotalOrders { get; set; }


        public int CompletedOrders { get; set; }


        public int CancelledOrders { get; set; }


        public double Rating { get; set; }


    }

}