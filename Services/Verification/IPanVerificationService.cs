using System.Threading.Tasks;

namespace Ecommerce.Services.Verification;

public class PanVerificationResult
{
    public bool IsValid { get; set; }
    public string PanStatus { get; set; } = "Valid";
    public string RegisteredName { get; set; } = "";
    public bool IsNameMatched { get; set; }
    public string Provider { get; set; } = "MOCK";
    public string Message { get; set; } = "";
}

public interface IPanVerificationService
{
    Task<PanVerificationResult> VerifyPanAsync(string panNumber, string submittedName);
}
