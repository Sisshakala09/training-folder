using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2.Models
{
    
        public partial class Booking
        {
            public int BookingId { get; set; }

            public int TripID { get; set; }

            public int PassengerID { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

            public string Status { get; set; } = null;
        }

    public partial class Driver
    {
        public int DriverId { get; set; }

        public string Phonenumber { get; set; } = null;

        public string Name { get; set; } = null;

        public string Status { get; set; } = null;
    }

}
