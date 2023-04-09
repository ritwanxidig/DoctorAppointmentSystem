using Data;
using Models;
using ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class ScheduleController : ControllerBase
{
    private readonly AppointmentsDbContext _dbContext;

    public ScheduleController(AppointmentsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var schedules = await _dbContext.Schedules
            .Include(s => s.TimeSlots)
            .Where(s => s.Doctor.UserId == User.GetId())
            .ToListAsync(HttpContext.RequestAborted);
        return Ok(schedules);
    }

    [HttpPost]
    public async Task<IActionResult> Add(ScheduleViewModel scheduleVM)
    {
        var doctor = await _dbContext.Doctors
            .Include(d => d.Schedules.Where(s=>s.Day == scheduleVM.Day))
            .SingleOrDefaultAsync(d => d.UserId == User.GetId());
        

        if (doctor is null)
        {
            return BadRequest("You are not a doctor");
        }

        if(doctor.IsVerified == false)
        {
            return BadRequest("You need to be verified first");
        }

        
        var schedule = new Schedule
        {
            Day = scheduleVM.Day,
            Location = scheduleVM.Location,
            DoctorId = doctor.Id,
            CreatedAt = DateTime.UtcNow,
            IsAvailable = true
        };
        await _dbContext.Schedules.AddAsync(schedule);
        await _dbContext.SaveChangesAsync(HttpContext.RequestAborted);
        return Created("", schedule);
    }




    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ModifyScheduleViewModel ScheduleVM)
    {
        var schedule = await _dbContext.Schedules.FindAsync(id);
        if (schedule is null) return BadRequest();

        // TODO: Only Owners of the schedule can update

        schedule.Location = ScheduleVM.Location;
        schedule.Day = ScheduleVM.Day;
        schedule.IsAvailable = ScheduleVM.IsAvailable;

        await _dbContext.SaveChangesAsync();
        return NoContent();
    }





    [HttpPost("{id}/timeslots")]
    public async Task<IActionResult> AddTimeSlot(int id, [FromBody] TimeSlotViewModel timeSlotVM)
    {
        var schedule = await _dbContext.Schedules.FindAsync(new object[] {id}, cancellationToken:HttpContext.RequestAborted);
        if (schedule == null) return BadRequest("This schedule doesnot exists!!");
        var timeslot = new TimeSlot
        {
            StartTime = timeSlotVM.StartTime,
            EndTime = timeSlotVM.EndTime,
            Description = timeSlotVM.Description,
            MaxAppointments = timeSlotVM.MaxAppointments,
            ScheduleId = schedule.Id,
            CreatedAt = DateTime.UtcNow
        };
        await _dbContext.TimeSlots.AddAsync(timeslot);
        await _dbContext.SaveChangesAsync();

        return Created("", timeslot);
    }

    [HttpPut("timeslots/{id}")]
    public async Task<IActionResult> UpdateTimeSlot(int id, [FromBody] TimeSlotViewModel timeSlotVM)
    {
        var timeslot = await _dbContext.TimeSlots.FindAsync(id);
        if (timeslot is null) return BadRequest();

        timeslot.StartTime = timeSlotVM.StartTime;
        timeslot.EndTime = timeSlotVM.EndTime;
        timeslot.Description = timeSlotVM.Description;
        timeslot.MaxAppointments = timeSlotVM.MaxAppointments;

        await _dbContext.SaveChangesAsync();
        return NoContent();
    }


}