using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public string StartDestination { get; set; } = null!;

    public string EndDestination { get; set; } = null!;

    public double Distance { get; set; }
}
