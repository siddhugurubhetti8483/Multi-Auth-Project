namespace MultiAuthAPI.DTOs
{
    public class RegisterDTO
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "Customer";
    }

}
