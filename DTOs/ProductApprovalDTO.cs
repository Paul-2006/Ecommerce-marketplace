namespace Ecommerce.DTOs
{
    public class ProductApprovalDTO
    {
        public int ProductId { get; set; }

        public int AdminId { get; set; }

        public string ApprovalStatus { get; set; } = null!;

        public string? Remarks { get; set; }
    }
}