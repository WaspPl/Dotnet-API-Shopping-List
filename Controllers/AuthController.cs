using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Zaliczeniowy4.Models;

namespace Zaliczeniowy4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest("Username already exists");
                }

                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest("Email already exists");
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    PasswordHash = passwordHash
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while registering the user", details = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid username or password");
                }

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while logging in", details = ex.Message });
            }
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:DurationInMinutes"])),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error generating JWT token: {ex.Message}", ex);
            }
        }
    }

    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
} 