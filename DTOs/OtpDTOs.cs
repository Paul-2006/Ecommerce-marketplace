namespace Ecommerce.DTOs
{
    public class OtpRequestDTO
    {
        public string Email { get; set; } = string.Empty;
        public string? Purpose { get; set; } = "Login";
    }

    public class OtpVerifyDTO
    {
        public string Email { get; set; } = string.Empty;
        public string OtpCode { get; set; } = string.Empty;
        public string? Purpose { get; set; } = "Login";
    }
}
