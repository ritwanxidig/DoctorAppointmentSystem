using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;

namespace Controllers;

[Route("[Controller]")]
public class UsersController : ControllerBase
{
    private readonly AppointmentsDbContext _context;
    public UsersController(AppointmentsDbContext context)
    {
        _context = context;
    }
    //Get All Users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.users.OrderBy(u => u.FullName).ToListAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] User user)
    {
        await _context.users.AddAsync(user);
        await _context.SaveChangesAsync();
        return Created("", user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _context.users.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] User user)
    {
        var targetUser = await _context.users.FindAsync(id);
        if (targetUser == null)
            return BadRequest();
        targetUser.FullName = user.FullName;
        targetUser.Address = user.Address;
        targetUser.Email = user.Email;
        targetUser.Gender = user.Gender;
        targetUser.PasswordHash = user.PasswordHash;

        _context.users.Update(targetUser);
       await _context.SaveChangesAsync();

        return NoContent();

    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.users.FindAsync(id);
        if (user == null)
            return BadRequest();
        _context.users.Remove(user);
       await _context.SaveChangesAsync();
        return NoContent();
    }
}