using Ecommerce.DTOs;

namespace Ecommerce.Services;

public interface IGeminiAiService
{
    Task<AiAssistantResponseDTO> ProcessCustomerQueryAsync(AiQueryDTO queryDto);
    Task<AiAssistantResponseDTO> CompareProductsAsync(List<int> productIds, string? userPrompt = null);
    Task<string> SummarizeProductReviewsAsync(int productId);
}
