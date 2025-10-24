using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuizApp.API.Data;
using QuizApp.API.Models;

namespace QuizApp.API.Services
{
    public class AuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        // -----------------------------
        // REGISTER USER
        // -----------------------------
        public async Task<bool> RegisterAsync(string username, string password, string role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
                return false; // username already exists

            var user = new User
            {
                Username = username,
                Role = role
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // -----------------------------
        // LOGIN USER
        // -----------------------------
        public async Task<string?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
                return null;

            // Use BCrypt to verify against seeded hash
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return GenerateJwtToken(user);
        }


        // -----------------------------
        // GENERATE JWT TOKEN
        // -----------------------------
        private string GenerateJwtToken(User user)
        {
            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT key is missing in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
