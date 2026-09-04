namespace Ecommerce.DTOs;

public class ProductDTO
{
    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Brand { get; set; }

    public string? Warranty { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? ApprovalStatus { get; set; }
}
