using System;
using System.Collections.Generic;

namespace ICA_10.Models;

public partial class Location
{
    public int Locationid { get; set; }

    public string LocationName { get; set; } = null!;

    public virtual ICollection<ItemsOffered> ItemsOffereds { get; } = new List<ItemsOffered>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
