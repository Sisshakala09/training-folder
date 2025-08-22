using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public string Model { get; set; } = null!;

    public double Capacity { get; set; }

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;
}
