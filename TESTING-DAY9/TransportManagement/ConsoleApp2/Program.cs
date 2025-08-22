using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2.Models;

namespace ConsoleApp2
{

    class Program
    {

        static void Main(string[] args)
        {
            CRUD c = new CRUD();
            Booking booking = new Booking();



            Console.Write("Please Enter BookingId:");
            booking.BookingId = int.Parse(Console.ReadLine());


            Console.Write("Please Enter TripID:");
            booking.TripID = int.Parse(Console.ReadLine());


            Console.Write("Please Enter PassengerID:");
            booking.PassengerID = int.Parse(Console.ReadLine());

            Console.Write("Please Enter Status:");
            booking.Status = Console.ReadLine();



            string message = c.AddBooking(booking);
            Console.WriteLine(message);
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            Console.WriteLine("Booking list");
            PrintList();



            Console.Write("Please enter BookingsId for deletion");
            int BookingsId = int.Parse(Console.ReadLine());
            string msg = c.DeleteBookings(BookingsId);
            Console.WriteLine(msg);
            PrintList();


            Booking b3 = new Booking();
            Console.WriteLine("Please enter the BookingId for update");
            b3.BookingId = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter the TripID for update");
            b3.TripID = int.Parse(Console.ReadLine());

            Console.WriteLine("Please enter the PassengerID for update");
            b3.PassengerID = int.Parse(Console.ReadLine());
            Console.WriteLine("Please enter the BookingDate for update");
            b3.BookingDate = DateTime.Parse(Console.ReadLine());

            Console.WriteLine("Please enter the Status for update");
            b3.Status = Console.ReadLine();
        }

        public static void PrintList()
        {
            CRUD c = new CRUD();

            List<Booking> bookings = c.GetBookings();
            Console.WriteLine("............................................................................");
            foreach (Booking b in bookings)
            {
                Console.WriteLine($"BookingId:{b.BookingId} | TripID:{b.TripID} | PassengerID:{b.PassengerID}|Status:{b.Status}");
            }
            Console.WriteLine("............................................................................");


            List<Driver> Drivers = c.GetDrivers();
            Console.WriteLine("............................................................................");
            foreach (Driver b in Drivers)
            {
                Console.WriteLine($"DriverId:{b.DriverId} | Phonenumber:{b.Phonenumber} | Name:{b.Name}|Status:{b.Status}");
            }
            Console.WriteLine("............................................................................");
        }




    }        
    }
    
