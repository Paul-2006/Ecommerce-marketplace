using System.Text.Json.Serialization;

namespace Ecommerce.Services.Verification;

public class GstVerificationResult
{
    public bool IsValid { get; set; }
    public string GstStatus { get; set; } = "Inactive"; // Active, Inactive, Cancelled
    public string LegalName { get; set; } = "";
    public string TradeName { get; set; } = "";
    public string BusinessAddress { get; set; } = "";
    public bool IsLegalNameMatched { get; set; }
    public bool IsTradeNameMatched { get; set; }
    public bool IsAddressMatched { get; set; }
    public string Provider { get; set; } = "MOCK";
    public string Message { get; set; } = "";
}

public interface IGstVerificationService
{
    Task<GstVerificationResult> VerifyGstAsync(string gstin, string submittedLegalName, string submittedTradeName, string submittedAddress);
}
