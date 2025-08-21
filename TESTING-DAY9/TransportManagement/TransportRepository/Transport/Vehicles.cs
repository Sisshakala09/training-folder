using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using TransportRepository.ITransport;

namespace TransportRepository.Transport
{
    public class TransportManagement : IVehicle
    {
        public bool AddVehicle(Vehicle vehicle) { return true; }
        public bool UpdateVehicle(Vehicle vehicle) { return true; }
        public bool DeleteVehicle(int vehicleId) { return true; }

        public bool ScheduleTrip(int vehicleId, int routeId, string departureDate, string arrivalDate) { return true; }
        public bool CancelTrip(int tripId) { return true; }

        public bool BookTrip(int tripId, int passengerId, string bookingDate) { return true; }
        public bool CancelBooking(int bookingId) { return true; }

        public bool AllocateDriver(int tripId, int driverId) { return true; }
        public bool DeallocateDriver(int tripId) { return true; }

        public List<Booking> GetBookingsByPassenger(int passengerId) { return new List<Booking>(); }
        public List<Booking> GetBookingsByTrip(int tripId) { return new List<Booking>(); }

        public List<Driver> GetAvailableDrivers() { return new List<Driver>(); }
    }
}
