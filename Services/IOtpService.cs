using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public interface IOtpService
    {
        Task<(bool Success, string Message, string OtpCode)> GenerateOtpAsync(string email, string purpose = "Login");
        Task<(bool Success, string Message)> VerifyOtpAsync(string email, string otpCode, string purpose = "Login");
    }
}
