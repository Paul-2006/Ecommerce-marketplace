using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public interface ISmsService
    {
        Task<bool> SendSmsAsync(string phoneNumber, string message);
        Task<bool> SendOtpSmsAsync(string phoneNumber, string recipientName, string otpCode, int expirationMinutes = 5);
    }
}
