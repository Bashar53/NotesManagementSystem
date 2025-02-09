using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Notes_Management_system.Model;
using Notes_Management_system.Services;
using Org.BouncyCastle.Crypto.Generators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Notes_Management_system.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController :ControllerBase
{
    
    //private readonly HttpClient _httpClient;

    //public AuthenticationController(IHttpClientFactory httpClient) => _httpClient = httpClient.CreateClient("EmployeeAPIBase");
    private readonly IConfiguration _configuration;
    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("name", user.Name),
            new Claim("email", user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2), // Token valid for 2 hours
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }






    [HttpPost("signup")]
    public IActionResult Signup([FromBody] User user)
    {
        var users = fileStorage.LoadUsers(); // Load users from your file storage service

        // Check if the user already exists
        if (users.Any(u => u.Email == user.Email))
        {
            return BadRequest(new { success = false, message = "User already exists" });
        }

        // Hash the password before saving
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        // Add the new user to the list
        users.Add(user);

        // Save the users back to file storage
        fileStorage.SaveUsers(users);

        // Return a successful response with JSON data
        return Ok(new { success = true, message = "User registered successfully." });
    }


    private static List<User> users = new List<User>();

    [HttpPost("login")]
     public IActionResult Login([FromBody] LoginRequest request)
     {
         var fileStorage = new FileStorageService();
         var users = fileStorage.LoadUsers().ToList();

         Console.WriteLine($"Searching for user: {request.Email}");

         

         // Log all users and their emails to verify correctness
         foreach (var u in users)
         {
             Console.WriteLine($"Loaded User: {u.Email}");
             Console.WriteLine($"Loaded User: {u.UserId}");
         }

        var user = users.FirstOrDefault();

         if (user == null)
         {
             Console.WriteLine("User not found!");
             return Unauthorized("User not found");
         }

         if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
         {
             Console.WriteLine("Invalid password!");
             return Unauthorized("Invalid password");
         }

         var token = GenerateJwtToken(user);
         return Ok(new { Token = token });
     } //OLD

    private readonly FileStorageService fileStorage = new FileStorageService();
    private const string SecretKey = "your_super_secret_key_that_is_very_secure_12345!"; // Replace with a secure key

    /*[HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var users = fileStorage.LoadUsers();
        var user = users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }

        // Generate JWT
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(SecretKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email) }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { Token = tokenHandler.WriteToken(token) });
    }
*/

    private static HashSet<string> revokedTokens = new HashSet<string>();

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            revokedTokens.Add(token);
        }
        return Ok(new { message = "Logged out successfully" });
    }

    public static bool IsTokenRevoked(string token)
    {
        return revokedTokens.Contains(token);
    }


}
