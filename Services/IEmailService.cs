using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task<bool> SendOtpEmailAsync(string toEmail, string recipientName, string otpCode, int expirationMinutes = 10);
    }
}
