using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Models;

namespace TransportManagement.Models
{
    public class TransportContext : DbContext
    {
        public TransportContext(DbContextOptions<TransportContext> options) : base(options) { }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Route> Routes { get; set; }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Passenger> Passengers { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Driver> Drivers { get; set; }
    }
}
