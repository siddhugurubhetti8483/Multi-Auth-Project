using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiAuthAPI.DTOs;
using MultiAuthAPI.Helpers;
using MultiAuthAPI.Services;

namespace MultiAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MfaController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly OtpService _otpService;
        private readonly JwtTokenGenerator _jwtGenerator;

        public MfaController(AuthService authService, OtpService otpService, JwtTokenGenerator jwtGenerator)
        {
            _authService = authService;
            _otpService = otpService;
            _jwtGenerator = jwtGenerator;
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] MfaLoginDTO dto)
        {
            var user = await _authService.ValidateUserOnly(dto);
            if (user == null)
                return Unauthorized("Invalid credentials");

            await _otpService.SendOtpAsync(user.Email);
            return Ok(new { message = "OTP sent to email" });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] MfaVerifyDTO dto)
        {
            var isValid = await _otpService.IsValidOtpAsync(dto.Email, dto.Otp);
            if (!isValid)
                return Unauthorized("Invalid or expired OTP");

            var user = await _authService.GetUserByEmail(dto.Email);
            var token = _jwtGenerator.GenerateToken(user);

            return Ok(new
            {
                message = "Login success with MFA",
                token
            });
        }
    }
}
