using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using ViewModels;
using Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Controllers;


[Route("[Controller]")]
[Authorize]

public class DoctorsController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public DoctorsController(AppointmentsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> getDoctors(int page, int size, string phone)
    {
        var query = _context.Doctors
            .Include(d => d.User)
            .Skip(page * size)
            .Take(size);
        if(!string.IsNullOrEmpty(phone))
        {
            query = query .Where(d => d.Phone == phone);
        }
        var doctors = await query
            
            .OrderBy(d => d.Id)
            .ToListAsync(HttpContext.RequestAborted);
        return Ok(doctors);
    }

    [HttpGet("{id}", Name = nameof(GetSingle))]
    public async Task<IActionResult> GetSingle(int id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        return Ok(doctor);
    }

    [HttpGet("specialties")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSpecialties()
    {
        var specialties = await _context.Doctors
            .GroupBy(d => d.Specialty)
            .Select(g => new
            {
                Name = g.Key,
                Count = g.Count()
            })
            .ToListAsync(HttpContext.RequestAborted);

        return Ok(specialties);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] DoctorViewModel doctorViewModel)
    {
        var Doctor = await _context.Doctors
            .SingleOrDefaultAsync(d => d.UserId == User.GetId(), HttpContext.RequestAborted);
        if(Doctor is not null)
        {
            return BadRequest("You are Already Doctor");
        }

        var doctor = new Doctor
        {
            Phone = doctorViewModel.Phone,
            Bio = doctorViewModel.Bio,
            Certificate = doctorViewModel.Certificate,
            Picture = doctorViewModel.Picture,
            Specialty = doctorViewModel.Specialty,
            TicketPrice = doctorViewModel.TicketPrice,
            UserId = User.GetId(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Doctors.AddAsync(doctor);
        await _context.SaveChangesAsync();
        return Created(nameof(GetSingle), doctor);
    }
}