using System.Threading.Tasks;

namespace Ecommerce.Services.Verification;

public class DigiLockerFetchResult
{
    public bool Success { get; set; }
    public string DocumentType { get; set; } = "";
    public string Uri { get; set; } = "";
    public string Issuer { get; set; } = "API SETU / DIGILOCKER";
    public string Status { get; set; } = "VERIFIED";
    public string Message { get; set; } = "";
}

public interface IDigiLockerService
{
    Task<DigiLockerFetchResult> FetchDocumentAsync(string documentType, string identifier);
}
