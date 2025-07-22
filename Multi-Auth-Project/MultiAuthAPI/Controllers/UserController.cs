using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiAuthAPI.Data;
using MultiAuthAPI.DTOs;

namespace MultiAuthAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsEmailVerified,
                    u.FailedLoginAttempts,
                    u.LockoutEnd

                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeUserRole([FromBody] RoleChangeDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound("User not found");

            // Validate role
            if (dto.NewRole != "Admin" && dto.NewRole != "Customer")
                return BadRequest("Invalid role. Must be 'Admin' or 'Customer'.");

            user.Role = dto.NewRole;
            await _context.SaveChangesAsync();

            return Ok($"Role updated to {dto.NewRole} for {dto.Email}");
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            // JWT token se extract kar rahe hain
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var fullName = User.FindFirst("FullName")?.Value;


            return Ok(new
            {
                Message = "You are authenticated",
                UserId = userId,
                Email = email,
                Role = role,
                FullName = fullName
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-panel")]
        public IActionResult AdminPanel()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin")
                return Forbid();
            return Ok("Welcome to admin panel. Only Admins can see this.");
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalAdmins = await _context.Users.CountAsync(u => u.Role == "Admin");
            var totalCustomers = await _context.Users.CountAsync(u => u.Role == "Customer");
            var verifiedUsers = await _context.Users.CountAsync(u => u.IsEmailVerified);
            var unverifiedUsers = await _context.Users.CountAsync(u => !u.IsEmailVerified);
            var faceRegisteredUsers = await _context.Users.CountAsync(u => u.FaceImageBase64 != null);

            var stats = new
            {
                totalUsers,
                totalAdmins,
                totalCustomers,
                verifiedUsers,
                unverifiedUsers,
                faceRegisteredUsers
            };

            return Ok(stats);
        }

    }
}
