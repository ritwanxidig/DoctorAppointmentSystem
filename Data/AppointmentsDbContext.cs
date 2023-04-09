using Data;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data;

public class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options)
    : base(options)
    {
        
    }
    public DbSet<User> users { get; set; }

    public DbSet<Doctor> Doctors { get; set; }

    public DbSet<Schedule> Schedules { get; set; }

	public DbSet<TimeSlot> TimeSlots { get; set; }

	public DbSet<Booking> Bookings { get; set; }

	public DbSet<BookingNote> Notes { get; set; }

	public DbSet<Review> Reviews { get; set; }
}