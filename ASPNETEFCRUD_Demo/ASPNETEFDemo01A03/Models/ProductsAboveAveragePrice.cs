using System;
using System.Collections.Generic;

namespace ASPNETEFDemo01A03.Models;

public partial class ProductsAboveAveragePrice
{
    public string ProductName { get; set; } = null!;

    public decimal? UnitPrice { get; set; }
}
