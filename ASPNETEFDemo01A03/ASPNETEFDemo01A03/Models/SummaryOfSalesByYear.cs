﻿using System;
using System.Collections.Generic;

namespace ASPNETEFDemo01A03.Models;

public partial class SummaryOfSalesByYear
{
    public DateTime? ShippedDate { get; set; }

    public int OrderId { get; set; }

    public decimal? Subtotal { get; set; }
}
