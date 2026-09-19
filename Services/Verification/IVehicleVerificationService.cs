using System;
using System.Threading.Tasks;

namespace Ecommerce.Services.Verification;

public class VehicleVerificationResult
{
    public bool IsValid { get; set; }
    public string RcStatus { get; set; } = "Active";
    public string RegisteredOwner { get; set; } = "";
    public string VehicleModel { get; set; } = "";
    public string InsuranceStatus { get; set; } = "Valid";
    public DateTime? InsuranceExpiryDate { get; set; }
    public bool IsInsuranceExpired { get; set; }
    public bool IsInsuranceExpiringSoon { get; set; }
    public string ExpiryStatus { get; set; } = "VALID"; // VALID, EXPIRING_SOON, EXPIRED
    public string Provider { get; set; } = "MOCK";
    public string Message { get; set; } = "";
}

public interface IVehicleVerificationService
{
    Task<VehicleVerificationResult> VerifyVehicleAsync(string vehicleNumber, string insurancePolicyNumber, DateTime? insuranceExpiry);
}
