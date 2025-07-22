using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Data;

namespace MultiAuthAPI.Services
{
    public class ForgotPasswordService
    {
        private readonly ApplicationDbContext _context;
        private readonly OtpService _otpService;

        public ForgotPasswordService(ApplicationDbContext context, OtpService otpService)
        {
            _context = context;
            _otpService = otpService;
        }
        public async Task<string> SendOtpAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return "User not found";

            return await _otpService.SendOtpAsync(email);
        }
        public async Task<string> ResetPasswordAsync(string email, string otp, string newPassword)
        {
            var isValid = await _otpService.IsValidOtpAsync(email, otp);
            if (!isValid)
                return "Invalid or expired OTP";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return "User not found";

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();

            return "Password reset successful";
        }
    }
}
