using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Data;
using MultiAuthAPI.Helpers;
using MultiAuthAPI.Models;

namespace MultiAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalLoginController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtTokenGenerator _tokenService;

        public ExternalLoginController(ApplicationDbContext context, JwtTokenGenerator tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var props = new AuthenticationProperties { RedirectUri = Url.Action("GoogleCallback") };
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync();
            if (!result.Succeeded)
                return Unauthorized();

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            // Check if user exists, if not create
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FullName = name,
                    Role = "Customer",
                    PasswordHash = "" // Google user
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            var token = _tokenService.GenerateToken(user);
            return Ok(new
            {
                message = "Login success using Google",
                token
            });
        }
    }
}
