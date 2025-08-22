using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string Phonenumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;
}
