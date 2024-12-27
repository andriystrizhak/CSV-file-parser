using System;
using System.Collections.Generic;

namespace CSVParser.Data;

public partial class Trip
{
    public int Id { get; set; }

    public DateTime PickupDatetime { get; set; }

    public DateTime DropoffDatetime { get; set; }

    public int? PassengerCount { get; set; }

    public double TripDistance { get; set; }

    public string StoreAndFwdFlag { get; set; } = null!;

    public int PULocationID { get; set; }

    public int DOLocationID { get; set; }

    public decimal FareAmount { get; set; }

    public decimal TipAmount { get; set; }
}
