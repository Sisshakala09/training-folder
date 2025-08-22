using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp2.Models.MyModelClasses;

namespace ConsoleApp2.Models
{
    
        public class CRUD
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ToString());
            public List<Booking> GetBookings()
            {
                List<Booking> bookinglist = new List<Booking>();
                SqlCommand cmd = new SqlCommand("select * from bookings", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
            da.Fill(dt);
                foreach (DataRow drow in dt.Rows)
                {
                    Booking b = new Booking()
                    {
                        BookingId = int.Parse(drow["BookingId"].ToString()),
                        TripID = int.Parse(drow["TripID"].ToString()),
                        PassengerID = int.Parse(drow["PassengerID"].ToString()),
                        Status = drow["Status"].ToString()
                    };
                    bookinglist.Add(b);
                }
                return bookinglist;
            }



        public List<Driver> GetDrivers()
        {
            List<Driver> driverlist = new List<Driver>();
            SqlCommand cmd = new SqlCommand("select * from Drivers", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            foreach (DataRow drow in dt.Rows)
            {
                Driver b = new Driver()
                {
                    DriverId = int.Parse(drow["DriverId"].ToString()),
                    Phonenumber = drow["Phonenumber"].ToString(),
                    Name = drow["Name"].ToString(),
                    Status = drow["Status"].ToString()
                };
                driverlist.Add(b);
            }
            return driverlist;
        }



        public string AddBooking(Booking booking)
        {
            SqlCommand cmd = new SqlCommand("Insert into bookings values('" + booking.TripID + "','" + booking.PassengerID + "','"+booking.BookingDate+"','" + booking.Status + "')", con);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                return "Record inserted successfully";
            }
            catch(Exception ex)
            {
                return "Booking insertion failed with error:" + ex.Message;
            }
            finally
            {
                con.Close();
            }
        }


        public string DeleteBookings(int BookingId)
        {
            SqlCommand cmd = new SqlCommand("Delete from bookings where BookingId='" + BookingId + "' ",con);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                return "Deletion successfully";
            }
            catch (Exception ex)
            {
                return "Deletion failed with error:" + ex.Message;
            }
            finally
            {
                con.Close();
            }
        }


        public string UpdateBooking(Booking booking)
        {
            SqlCommand cmd = new SqlCommand("Update bookings BookingId='"+ booking.BookingId + "','" + booking.TripID + "','" + booking.PassengerID + "','" + booking.BookingDate + "','" + booking.Status + "')", con);
            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
                return "Updation successfully";
            }
            catch (Exception ex)
            {
                return "Updation failed with error:" + ex.Message;
            }
            finally
            {
                con.Close();
            }
        }
    }
    }

