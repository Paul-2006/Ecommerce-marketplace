namespace Ecommerce.DTOs
{

    public class ProductReviewDTO
    {

        public int CustomerId { get; set; }


        public int ProductId { get; set; }


        public int Rating { get; set; }


        public string ReviewText { get; set; } = null!;


    }

}