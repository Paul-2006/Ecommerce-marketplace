using System;
using System.Threading.Tasks;

namespace Ecommerce.Services.Verification;

public class DrivingLicenceVerificationResult
{
    public bool IsValid { get; set; }
    public string LicenceStatus { get; set; } = "Active";
    public string HolderName { get; set; } = "";
    public string VehicleClass { get; set; } = "LMV / Two Wheeler";
    public DateTime? ExpiryDate { get; set; }
    public bool IsExpired { get; set; }
    public bool IsExpiringSoon { get; set; }
    public string ExpiryStatus { get; set; } = "VALID"; // VALID, EXPIRING_SOON, EXPIRED
    public string Provider { get; set; } = "MOCK";
    public string Message { get; set; } = "";
}

public interface IDrivingLicenceVerificationService
{
    Task<DrivingLicenceVerificationResult> VerifyDrivingLicenceAsync(string dlNumber, string submittedName, DateTime? expiryDate);
}
