namespace Ecommerce.DTOs;

public class AiAssistantResponseDTO
{
    public string Answer { get; set; } = string.Empty;
    public List<AiProductRecommendationCardDTO> RecommendedProducts { get; set; } = new();
    public string Intent { get; set; } = "General";
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
}

public class AiProductRecommendationCardDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public double Rating { get; set; }
    public int ReviewCount { get; set; }
    public int StockQuantity { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RecommendationReason { get; set; } = string.Empty;
}
