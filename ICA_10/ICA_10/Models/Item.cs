using System;
using System.Collections.Generic;

namespace ICA_10.Models;

public partial class Item
{
    public int Itemid { get; set; }

    public string ItemName { get; set; } = null!;

    public double ItemPrice { get; set; }

    public virtual ICollection<ItemsOffered> ItemsOffereds { get; } = new List<ItemsOffered>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
