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
        private readonly MailService _mailService;

        public AuthenticationController(VisionManagementContext context, JwtService jwtService, MailService mailService)
        {
            _context = context;
            _jwtService = jwtService;
            _mailService = mailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
                return BadRequest("Username is already taken");

            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
                return BadRequest("Email is already in use");

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "User");
            if (role == null)
                return BadRequest("Default role not found");

            string otpCode = new Random().Next(100000, 999999).ToString();

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                RoleId = role.RoleId,
                OtpCode = otpCode,
                OtpExpiration = DateTime.UtcNow.AddMinutes(10),
                IsOtpVerified = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await _mailService.SendOtpEmail(user.Email, otpCode);

            return Ok(new { message = "User registered. OTP sent to your email." });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return BadRequest("User not found");

            if (user.IsOtpVerified)
                return BadRequest("User is already verified.");

            if (user.OtpCode != dto.Otp || user.OtpExpiration < DateTime.UtcNow)
                return BadRequest("Invalid or expired OTP.");

            user.IsOtpVerified = true;
            user.OtpCode = null;
            user.OtpExpiration = null;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "OTP verified. You can now log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            if (!user.IsOtpVerified)
                return Unauthorized("Please verify your email using the OTP before logging in.");

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                username = user.Username,
                role = user.Role.RoleName
            });
        }

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

    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class VerifyOtpDto
    {
        public string Email { get; set; }
        public string Otp { get; set; }
    }
}
