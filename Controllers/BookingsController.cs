using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Data;
using Models;
using ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Helpers;

namespace Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public BookingsController(AppointmentsDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _context.Bookings.ToListAsync();
        return Ok(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] BookingViewModel bookingVM)
    {
        var timeSlot = await _context.TimeSlots
            .Include(ts => ts.Schedule)
            .ThenInclude(s => s.Doctor)
            .SingleOrDefaultAsync(ts => ts.Id == bookingVM.TimeSlotId);
        if (timeSlot is null) return BadRequest("Selected time could not be recognized");

        if (bookingVM.AppointmentTime < DateTime.Today)
        {
            return BadRequest($"You can't make an appointment at past time");
        }

        if (bookingVM.AppointmentTime.DayOfWeek != timeSlot.Schedule.Day)
        {
            return BadRequest($"Doctor is not availabe on {bookingVM.AppointmentTime.DayOfWeek}");
        }

        if(timeSlot.MaxAppointments <= timeSlot.Bookings.Count)
        {
            return BadRequest("Wuu Kaa buuxa maanta");
        }


        var ticketPrice = timeSlot.Schedule.Doctor.TicketPrice;
        var rate = 0.02m;
        var commission = ticketPrice * rate;


        // TODO: Add Payment Gateway Like (eDahab, Zaad ...)
        var Transactionid = new Random().Next(1_000, 9_000);
        var booking = new Booking
        {
            AppointmentTime = bookingVM.AppointmentTime,
            IsCompleted = false,
            UserId = User.GetId(),
            CreatedAt = DateTime.UtcNow,
            TransactionId = "TR" + Transactionid,
            PaidAmount = 10,
            Commission = commission,
            DoctorRevenue = ticketPrice - commission,
            PaymentMethod = bookingVM.PaymentMethod,
            TimeSlotId = timeSlot.Id
        };

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        return Created("", booking);
    }
}