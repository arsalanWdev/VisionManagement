using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VisionManagement.Models;
using VisionManagement.Services;

namespace VisionManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly VisionManagementContext _context;
        private readonly JwtService _jwtService;

        public AuthenticationController(VisionManagementContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // ================== LOGIN ONLY ==================
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                username = user.Username,
                role = user.Role.RoleName
            });
        }

        // ================== HELPER FUNCTIONS ==================
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }

    // DTOs
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
