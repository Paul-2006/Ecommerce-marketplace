namespace Ecommerce.DTOs
{
    public class FindAccountDTO
    {
        public string Identifier { get; set; } = string.Empty;
    }

    public class SendForgotOtpDTO
    {
        public string Identifier { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string DeliveryMethod { get; set; } = "email"; // "email" or "sms"
        public string Channel { get; set; } = "email"; // "email" or "sms"
    }

    public class VerifyForgotOtpDTO
    {
        public string Identifier { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class ResetPasswordDTO
    {
        public string Identifier { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public string OtpCode { get; set; } = string.Empty;
        public string ResetToken { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
