namespace Ecommerce.DTOs
{

    public class ProductRecommendationDTO
    {

        public int CustomerId { get; set; }


        public int ProductId { get; set; }


        public string Reason { get; set; } = null!;


    }

}