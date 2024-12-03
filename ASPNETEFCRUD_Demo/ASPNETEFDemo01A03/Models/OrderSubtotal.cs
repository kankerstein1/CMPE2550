using System;
using System.Collections.Generic;

namespace ASPNETEFDemo01A03.Models;

public partial class OrderSubtotal
{
    public int OrderId { get; set; }

    public decimal? Subtotal { get; set; }
}
