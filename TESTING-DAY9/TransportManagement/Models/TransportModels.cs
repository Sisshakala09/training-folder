using System.Collections.Specialized;

namespace Models
{
   
    public class Vehicle
    {
        public int VehicleId { get; set; }
        public  string Model { get; set; }
        public double Capacity { get; set; }

        public  string Type { get; set; }  // e.g., Bus, Truck, Taxi
        public  string Status { get; set; }
    }
    public class Route
    {
        public int RouteId { get; set; }
        public  string StartDestination { get; set; }
        public  string EndDestination { get; set; }
        public double Distance { get; set; }
    }

    public class Trip
    {
        public int TripId { get; set; }
        public int VehicleID { get; set; }
        public int RouteID { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public  string Status { get; set; }
        public  string TripType { get; set; }
        public int MaxPassengers { get; set; }
    }

    public class Passenger
    {
        public int PassengerID { get; set; }
        public  string FirstName { get; set; }
        public  string gender { get; set; }
        public int age { get; set; }
        public  string Email { get; set; }
        public int PhoneNumber { get; set; }

    }

    public class Booking
    {
        public int BookingId { get; set; }
        public int TripID { get; set; }
        public int PassengerID { get; set; }
        public DateTime BookingDate { get; set; }
        public  string Status { get; set; }
    }

    public class Driver
    {
        public int DriverId { get; set; }
        //public string Model { get; set; }
        public string Phonenumber { get; set; }

        public string Name { get; set; }  // e.g., Bus, Truck, Taxi
        public string Status { get; set; }
    }
}
