namespace MultiAuthAPI.DTOs
{
    public class MfaVerifyDTO
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
