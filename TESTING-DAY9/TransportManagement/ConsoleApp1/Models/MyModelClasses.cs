using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    
        public partial class Booking
        {
            public int BookingId { get; set; }

            public int TripID { get; set; }

            public int PassengerID { get; set; }

            public DateTime BookingDate { get; set; }

            public string Status { get; set; } = null!;
        }
    }
}
