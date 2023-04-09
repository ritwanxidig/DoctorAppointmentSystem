using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Controllers;

[Route("[controller]")]

public class AuthController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public AuthController(AppointmentsDbContext context)
    {
        _context = context;
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(string email)
    {

        var user = await _context.users.SingleOrDefaultAsync(u => u.Email == email);
        if(user is null)
        {
            return BadRequest("Invalid Login Attempt");
        }

        var keyInput = "Ridwan_Abdirashid_Mohamed_Haid_Gouled_Mohamed";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyInput));


        var now = DateTime.Now;

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new ("Fullname", user.FullName),
            new ("email", user.Email),
            new ("gender", user.Gender)
        };

        var doctor = await _context.Doctors.SingleOrDefaultAsync(d=>d.UserId == user.Id);
        if(doctor is not null)
        {
            claims.Add(new("doctorId", doctor.Id.ToString()));
        }

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken("MyAPI", "MyFrontEnd", claims, now, now.AddHours(1), credentials);

        var handler = new JwtSecurityTokenHandler();
        var Jwt = handler.WriteToken(token);
        var result = new
        {
            token = Jwt
        };
        return Ok(result);
    }
}