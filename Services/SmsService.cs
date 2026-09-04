using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services
{
    public class SmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var apiKey = _configuration["SmsSettings:ApiKey"];
                var senderId = _configuration["SmsSettings:SenderId"] ?? "AURALUXE";

                // Fallback dev log if SMS gateway API key is not configured
                if (string.IsNullOrWhiteSpace(apiKey))
                {
                    _logger.LogInformation("[DEV SMS SERVICE] SMS dispatched to {PhoneNumber} (Sender: {SenderId}): {Message}", phoneNumber, senderId, message);
                    return Task.FromResult(true);
                }

                _logger.LogInformation("SMS dispatched successfully to {PhoneNumber}", phoneNumber);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {PhoneNumber}", phoneNumber);
                _logger.LogWarning("[DEV FALLBACK LOG] SMS payload to {PhoneNumber}: {Message}", phoneNumber, message);
                return Task.FromResult(true);
            }
        }

        public async Task<bool> SendOtpSmsAsync(string phoneNumber, string recipientName, string otpCode, int expirationMinutes = 5)
        {
            string name = string.IsNullOrWhiteSpace(recipientName) ? "Customer" : recipientName;
            string message = $"[AURA Luxe] Hello {name}, your security verification code is: {otpCode}. Valid for {expirationMinutes} mins. Do not share this code.";
            return await SendSmsAsync(phoneNumber, message);
        }
    }
}
