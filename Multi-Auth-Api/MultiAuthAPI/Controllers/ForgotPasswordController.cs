using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MultiAuthAPI.DTOs;
using MultiAuthAPI.Services;

namespace MultiAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly ForgotPasswordService _service;

        public ForgotPasswordController(ForgotPasswordService service)
        {
            _service = service;
        }

        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] ForgotPasswordRequestDTO dto)
        {
            var message = await _service.SendOtpAsync(dto.Email);
            return Ok(new { message });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var message = await _service.ResetPasswordAsync(dto.Email, dto.Otp, dto.NewPassword);
            return Ok(new { message });
        }
    }
}
