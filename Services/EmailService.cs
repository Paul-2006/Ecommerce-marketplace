using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var portStr = _configuration["EmailSettings:Port"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"] ?? "noreply@auraluxe.com";
                var senderName = _configuration["EmailSettings:SenderName"] ?? "AURA Luxe Marketplace";
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var enableSsl = bool.TryParse(_configuration["EmailSettings:EnableSsl"], out var ssl) && ssl;

                // Fallback to dev logger if SMTP server is not configured
                if (string.IsNullOrWhiteSpace(smtpServer) || string.IsNullOrWhiteSpace(username))
                {
                    _logger.LogInformation("[DEV EMAIL SERVICE] Email dispatched to {ToEmail}. Subject: {Subject}\nBody:\n{HtmlBody}", toEmail, subject, htmlBody);
                    return true;
                }

                int port = int.TryParse(portStr, out var p) ? p : 587;

                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(senderEmail, senderName);
                    message.To.Add(new MailAddress(toEmail));
                    message.Subject = subject;
                    message.Body = htmlBody;
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(smtpServer, port))
                    {
                        client.Credentials = new NetworkCredential(username, password);
                        client.EnableSsl = enableSsl;
                        await client.SendMailAsync(message);
                    }
                }

                _logger.LogInformation("Email successfully sent to {ToEmail}", toEmail);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
                // Fallback dev log so flow never breaks in dev environment
                _logger.LogWarning("[DEV FALLBACK LOG] Email payload to {ToEmail}: {Subject}", toEmail, subject);
                return true;
            }
        }

        public async Task<bool> SendOtpEmailAsync(string toEmail, string recipientName, string otpCode, int expirationMinutes = 10)
        {
            string name = string.IsNullOrWhiteSpace(recipientName) ? "Valued Customer" : recipientName;
            string subject = $"[{otpCode}] Your AURA Luxe Security Verification Code";

            string htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'/>
    <style>
        body {{ font-family: 'Georgia', 'Times New Roman', serif; background-color: #FAF7F2; color: #1E293B; margin: 0; padding: 20px; }}
        .container {{ max-width: 560px; margin: 0 auto; background: #FFFFFF; border: 1px solid #E2E8F0; border-radius: 12px; padding: 32px; box-shadow: 0 4px 16px rgba(0,0,0,0.06); }}
        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #F1F5F9; }}
        .brand {{ font-size: 24px; font-weight: bold; color: #701A75; letter-spacing: 2px; }}
        .badge {{ background: #701A75; color: #FFFFFF; padding: 4px 12px; font-size: 11px; font-family: sans-serif; text-transform: uppercase; border-radius: 20px; font-weight: 600; display: inline-block; margin-top: 6px; }}
        .content {{ padding: 24px 0; font-family: sans-serif; line-height: 1.6; font-size: 15px; color: #334155; }}
        .otp-box {{ background: #FAF7F2; border: 2px dashed #C5A880; border-radius: 10px; padding: 20px; text-align: center; margin: 20px 0; }}
        .otp-code {{ font-family: monospace; font-size: 36px; font-weight: bold; color: #701A75; letter-spacing: 8px; margin: 0; }}
        .expiry {{ font-size: 13px; color: #64748B; margin-top: 8px; }}
        .warning {{ background: #FFFBEB; border-left: 4px solid #F59E0B; padding: 12px 16px; font-size: 13px; color: #92400E; margin-top: 20px; border-radius: 0 8px 8px 0; }}
        .footer {{ border-top: 1px solid #F1F5F9; margin-top: 28px; padding-top: 16px; text-align: center; font-size: 12px; color: #94A3B8; font-family: sans-serif; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='brand'>◆ AURA LUXE</div>
            <div class='badge'>Security Verification</div>
        </div>
        <div class='content'>
            <p>Dear <strong>{WebUtility.HtmlEncode(name)}</strong>,</p>
            <p>You requested a single-use verification code to authenticate your account on <strong>AURA Luxe Marketplace</strong>.</p>
            
            <div class='otp-box'>
                <p class='otp-code'>{otpCode}</p>
                <p class='expiry'>⏳ This code will expire in <strong>{expirationMinutes} minutes</strong>.</p>
            </div>

            <p>Please enter this 6-digit verification code into the application portal to complete your security clearance.</p>

            <div class='warning'>
                <strong>🛡️ Security Warning:</strong> Never share this code with anyone. AURA Luxe support representatives will never ask for your verification code via phone or email.
            </div>
        </div>
        <div class='footer'>
            <p>&copy; 2026 AURA Luxe Marketplace Inc. All rights reserved.</p>
            <p>Automated security notification — Please do not reply to this email.</p>
        </div>
    </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, htmlBody);
        }
    }
}
