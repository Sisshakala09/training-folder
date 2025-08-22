using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ConsoleApp1.Models
{
           public class CRUD
        {
            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnection"].ToString());
            public List<Booking> GetBookings()
            {
                List<Booking> bookinglist = new List<Booking>();
                SqlCommand cmd = new SqlCommand("select * from booking", con);
                SqlDataAdapter da new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach(DataRow drow in dt.Rows)
                {
                    bookinglist b = new bookinglist()
                    {
                        BookingId = int.Parse(drow["BookingId"].ToString()),
                        TripID = int.Parse(drow["TripID"].ToString()),
                        PassengerID = drow["PassengerID"].ToString(),
                        Status = drow["Status"].ToString()
                    };
                    bookinglist.Add(b);
                }
                return bookinglist;
            }
    }
}
