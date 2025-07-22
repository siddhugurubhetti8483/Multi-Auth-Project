using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Data;
using MultiAuthAPI.Models;

namespace MultiAuthAPI.Services
{
    public class OtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public OtpService(ApplicationDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<string> SendOtpAsync(string email)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();
            var otpEntry = new OtpEntry
            {
                Email = email,
                Code = otpCode,
                ExpiryTime = DateTime.Now.AddMinutes(2)
            };

            _context.OtpEntries.Add(otpEntry);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(email, "Your OTP Code", $"Your OTP is: {otpCode}");

            return "OTP sent to email";
        }

        public async Task<bool> VerifyOtpAsync(string email, string code)
        {
            var record = await _context.OtpEntries
                .Where(x => x.Email == email && x.Code == code)
                .OrderByDescending(x => x.ExpiryTime)
                .FirstOrDefaultAsync();

            if (record == null || record.ExpiryTime < DateTime.Now)
                return false;

            return true;
        }
        public async Task<bool> IsValidOtpAsync(string email, string otp)
        {
            var record = await _context.OtpEntries
                .Where(x => x.Email == email && x.Code == otp)
                .OrderByDescending(x => x.ExpiryTime)
                .FirstOrDefaultAsync();

            if (record == null || record.ExpiryTime < DateTime.Now)
                return false;

            return true;
        }

    }
}
