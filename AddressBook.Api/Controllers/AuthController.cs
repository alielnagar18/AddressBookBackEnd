using Microsoft.AspNetCore.Mvc;
using AddressBook.Infrastructure.Data;
using AddressBook.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.Data;
using AddressBook.Core.DTOs;

namespace AddressBook.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AuthDto req)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check if username already exists
        var existing = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (existing != null)
            return BadRequest(new { message = "Username already taken" });

        var hashed = BCrypt.Net.BCrypt.HashPassword(req.Password);
        var user = new User
        {
            Username = req.Username,
            PasswordHash = hashed
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "User registered successfully" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AuthDto req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Invalid credentials");

        var token = GenerateJwtToken(user);
        return Ok(new { token });
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
