using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiAuthAPI.DTOs;
using MultiAuthAPI.Services;

namespace MultiAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtpController : ControllerBase
    {
        private readonly OtpService _otpService;

        public OtpController(OtpService otpService)
        {
            _otpService = otpService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpDTO dto)
        {
            var result = await _otpService.SendOtpAsync(dto.Email);
            return Ok(new { message = result });
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO dto)
        {
            var success = await _otpService.VerifyOtpAsync(dto.Email, dto.Otp);
            return Ok(new { verified = success });
        }
    }
}
