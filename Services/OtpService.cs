using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services
{
    public class OtpRecord
    {
        public string HashedOtp { get; set; } = string.Empty;
        public DateTime ExpirationTime { get; set; }
        public int AttemptCount { get; set; }
        public int RequestCount { get; set; }
        public DateTime LastRequestTime { get; set; }
        public bool IsVerified { get; set; }
    }

    public class OtpService : IOtpService
    {
        private static readonly ConcurrentDictionary<string, OtpRecord> OtpStore = new();
        private readonly ILogger<OtpService> _logger;

        private const int ExpirationMinutes = 5;
        private const int MaxAttempts = 5;
        private const int MaxResendRequests = 3;

        public OtpService(ILogger<OtpService> logger)
        {
            _logger = logger;
        }

        public Task<(bool Success, string Message, string OtpCode)> GenerateOtpAsync(string email, string purpose = "Login")
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return Task.FromResult((false, "Email address is required.", string.Empty));
            }

            var cleanEmail = email.Trim().ToLowerInvariant();
            var key = $"{cleanEmail}:{purpose}";
            var now = DateTime.UtcNow;

            if (OtpStore.TryGetValue(key, out var existing))
            {
                // Rate limit resend requests within 15 minutes window
                if (now.Subtract(existing.LastRequestTime).TotalMinutes < 15)
                {
                    if (existing.RequestCount >= MaxResendRequests)
                    {
                        return Task.FromResult((false, "Too many OTP requests. Please wait 15 minutes before requesting a new code.", string.Empty));
                    }
                }
                else
                {
                    existing.RequestCount = 0;
                }
            }

            // Cryptographically secure 6-digit OTP generation
            int randomNumber = RandomNumberGenerator.GetInt32(100000, 1000000);
            string otpCode = randomNumber.ToString();
            string hashedOtp = HashOtp(otpCode);

            var record = existing ?? new OtpRecord();
            record.HashedOtp = hashedOtp;
            record.ExpirationTime = now.AddMinutes(ExpirationMinutes);
            record.AttemptCount = 0;
            record.RequestCount = (existing?.RequestCount ?? 0) + 1;
            record.LastRequestTime = now;
            record.IsVerified = false;

            OtpStore[key] = record;

            _logger.LogInformation("Cryptographically secure OTP generated for key {Key}. Expiration: {Exp}", key, record.ExpirationTime);

            return Task.FromResult((true, "OTP generated successfully.", otpCode));
        }

        public Task<(bool Success, string Message)> VerifyOtpAsync(string email, string otpCode, string purpose = "Login")
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(otpCode))
            {
                return Task.FromResult((false, "Email address and verification code are required."));
            }

            var cleanEmail = email.Trim().ToLowerInvariant();
            var key = $"{cleanEmail}:{purpose}";
            var now = DateTime.UtcNow;

            if (!OtpStore.TryGetValue(key, out var record) || record.IsVerified)
            {
                return Task.FromResult((false, "Invalid or expired verification code. Please request a new code."));
            }

            if (now > record.ExpirationTime)
            {
                OtpStore.TryRemove(key, out _);
                return Task.FromResult((false, "Verification code expired. Please request a new code."));
            }

            if (record.AttemptCount >= MaxAttempts)
            {
                OtpStore.TryRemove(key, out _);
                return Task.FromResult((false, "Too many incorrect attempts. This verification code has been invalidated. Please request a new code."));
            }

            record.AttemptCount++;

            string targetHash = HashOtp(otpCode.Trim());
            if (record.HashedOtp != targetHash)
            {
                int remaining = MaxAttempts - record.AttemptCount;
                return Task.FromResult((false, $"Invalid verification code. {remaining} attempt(s) remaining."));
            }

            // Mark single-use and remove record
            record.IsVerified = true;
            OtpStore.TryRemove(key, out _);

            _logger.LogInformation("OTP successfully verified and invalidated for key {Key}.", key);
            return Task.FromResult((true, "Verification successful."));
        }

        private static string HashOtp(string otp)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(otp + "AuraLuxeSalt2026"));
            return Convert.ToBase64String(bytes);
        }
    }
}
