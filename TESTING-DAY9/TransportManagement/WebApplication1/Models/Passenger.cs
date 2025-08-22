using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Passenger
{
    public int PassengerID { get; set; }

    public string FirstName { get; set; } = null!;

    public string gender { get; set; } = null!;

    public int age { get; set; }

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
}
