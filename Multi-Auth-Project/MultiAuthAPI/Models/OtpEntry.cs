namespace MultiAuthAPI.Models
{
    public class OtpEntry
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Code { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
