using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Trip
{
    public int TripId { get; set; }

    public int VehicleID { get; set; }

    public int RouteID { get; set; }

    public DateTime DepartureDate { get; set; }

    public DateTime ArrivalDate { get; set; }

    public string Status { get; set; } = null!;

    public string TripType { get; set; } = null!;

    public int MaxPassengers { get; set; }
}
