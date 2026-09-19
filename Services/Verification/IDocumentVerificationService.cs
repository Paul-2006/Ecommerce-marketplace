using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ecommerce.Services.Verification;

public class DocumentUploadResult
{
    public bool Success { get; set; }
    public string FilePath { get; set; } = "";
    public string DocumentType { get; set; } = "";
    public string FileType { get; set; } = "";
    public long FileSize { get; set; }
    public string ExtractedOcrText { get; set; } = "";
    public string ExtractedDataJson { get; set; } = "";
    public string Message { get; set; } = "";
}

public interface IDocumentVerificationService
{
    Task<DocumentUploadResult> UploadAndExtractDocumentAsync(IFormFile file, string documentType, string ownerType, int ownerId);
}
