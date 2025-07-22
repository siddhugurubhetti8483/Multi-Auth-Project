using System.Text;
using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Data;
using MultiAuthAPI.DTOs;
using MultiAuthAPI.Helpers;
using MultiAuthAPI.Models;

namespace MultiAuthAPI.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly EmailService _emailService;


        public AuthService(ApplicationDbContext context, JwtTokenGenerator jwtTokenGenerator, EmailService emailService)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
        }

        public async Task<string> RegisterUser(RegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return "Email already registered";

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                Role = dto.Role ?? "Customer",
                IsEmailVerified = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Send verification email
            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Email));
            var verifyUrl = $"https://localhost:7195/api/auth/verify-email?token={token}";
            var body = $"Hello {user.FullName},<br><br>Please verify your email by clicking the link below:<br><a href='{verifyUrl}'>Verify Email</a>";

            await _emailService.SendEmailAsync(user.Email, "Email Verification", body);

            return "Registered successfully! Please verify your email.";
        }

        public async Task<User> ValidateUser(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            // 🔐 Check if locked
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
                throw new Exception($"Account locked until {user.LockoutEnd.Value.ToLocalTime()}");

            // ✅ Check password
            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordCorrect)
            {
                user.FailedLoginAttempts += 1;

                if (user.FailedLoginAttempts >= 3)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(5);
                    await _context.SaveChangesAsync();
                    throw new Exception("Account locked for 5 minutes due to multiple failed attempts.");
                }

                await _context.SaveChangesAsync();
                return null;
            }

            // 👍 Password correct → reset
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            await _context.SaveChangesAsync();

            // 🔍 Is email verified?
            if (!user.IsEmailVerified)
                throw new Exception("Email not verified");

            return user;
        }
        public async Task<User?> ValidateUserOnly(MfaLoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        //public async Task<(string accessToken, string refreshToken)> LoginAsync(string email, string password)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        //        return (null, null);

        //    var accessToken = _jwtTokenGenerator.GenerateToken(user);
        //    var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

        //    user.RefreshToken = refreshToken;
        //    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        //    await _context.SaveChangesAsync();

        //    return (accessToken, refreshToken);
        //}
       



    }
}
