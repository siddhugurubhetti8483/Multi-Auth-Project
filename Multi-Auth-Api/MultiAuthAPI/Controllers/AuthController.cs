using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Data;
using MultiAuthAPI.DTOs;
using MultiAuthAPI.Helpers;
using MultiAuthAPI.Services;

namespace MultiAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public AuthController(AuthService authService, JwtTokenGenerator jwtTokenGenerator, ApplicationDbContext context, EmailService emailService)
        {
            _authService = authService;
            _jwtTokenGenerator = jwtTokenGenerator;
            _context = context;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _authService.RegisterUser(dto);
            return Ok(new { message = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            try
            {
                var user = await _authService.ValidateUser(dto);
                if (user == null)
                    return Unauthorized("Invalid email or password");

                // ✅ If user is valid → issue tokens
                var accessToken = _jwtTokenGenerator.GenerateToken(user);
                var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                user.FailedLoginAttempts = 0;               // Reset on success
                user.LockoutEnd = null;                     // Reset lock
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Login successful",
                    token = accessToken,
                    refreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new
                {
                    message = "Login failed",
                    error = ex.Message // e.g., "Account locked until...", "Email not verified"
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("User not found");

            if (user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token");

            var newAccessToken = _jwtTokenGenerator.GenerateToken(user);
            var newRefreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                token = newAccessToken,
                refreshToken = newRefreshToken
            });
        }

        [HttpPost("upload-face")]
        public async Task<IActionResult> UploadFace([FromBody] FaceUploadDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return NotFound("User not found");

            user.FaceImageBase64 = dto.Base64Image;
            await _context.SaveChangesAsync();

            return Ok("Face image uploaded successfully");
        }

        [HttpPost("face-login")]
        public async Task<IActionResult> FaceLogin([FromBody] FaceLoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || string.IsNullOrEmpty(user.FaceImageBase64))
                return Unauthorized("Face not registered");

            float similarity = FaceRecognitionHelper.CompareImages(user.FaceImageBase64, dto.Base64Image);

            if (similarity >= 0.85f) // threshold
            {
                var token = _jwtTokenGenerator.GenerateToken(user);
                return Ok(new { token });
            }

            return Unauthorized("Face mismatch");
        }

        [HttpGet("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            try
            {
                var email = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                    return NotFound("User not found");

                if (user.IsEmailVerified)
                    return Ok("Email already verified");

                user.IsEmailVerified = true;
                await _context.SaveChangesAsync();

                return Ok("Email verified successfully!");
            }
            catch
            {
                return BadRequest("Invalid or corrupted token.");
            }
        }

        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound("User not found");

            if (user.IsEmailVerified)
                return BadRequest("Email already verified");

            var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(email));
            var verificationUrl = $"https://localhost:7195/api/Auth/verify-email?token={token}";

            await _emailService.SendEmailAsync(email, "Verify Your Email", $"Click to verify: {verificationUrl}");

            return Ok("Verification email re-sent.");
        }





    }
}
