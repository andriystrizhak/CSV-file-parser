using CsvHelper.Configuration.Attributes;

namespace CSVParser.Models;

public record TripRecord
{
    private static readonly TimeZoneInfo EstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    private DateTime _tpepPickupDatetime;
    public DateTime TpepPickupDatetime
    {
        get => _tpepPickupDatetime;
        set => _tpepPickupDatetime = ConvertToUtc(value);
    }

    private DateTime _tpepDropoffDatetime;
    public DateTime TpepDropoffDatetime
    {
        get => _tpepDropoffDatetime;
        set => _tpepDropoffDatetime = ConvertToUtc(value);
    }

    private string _storeAndFwdFlag = string.Empty;
    public string StoreAndFwdFlag
    {
        get => _storeAndFwdFlag;
        set => _storeAndFwdFlag = ConvertStoreAndFwdFlag(value);
    }

    public int? PassengerCount { get; set; }
    public decimal TripDistance { get; set; }
    public int PULocationID { get; set; }
    public int DOLocationID { get; set; }
    public decimal FareAmount { get; set; }
    public decimal TipAmount { get; set; }

    // Method for checking for duplicates
    public bool IsDuplicate(TripRecord other)
    {
        return TpepPickupDatetime == other.TpepPickupDatetime &&
               TpepDropoffDatetime == other.TpepDropoffDatetime &&
               PassengerCount == other.PassengerCount;
    }

    // Overriding ToString for convenience
    public override string ToString()
    {
        return $"Pickup: {TpepPickupDatetime}, Dropoff: {TpepPickupDatetime}, " +
               $"Passengers: {PassengerCount}, Distance: {TripDistance}, " +
               $"PULocation: {PULocationID}, DOLocation: {DOLocationID}, " +
               $"Fare: {FareAmount}, Tip: {TipAmount}, " +
               $"Store and Forward: {StoreAndFwdFlag}";
    }

    // Method for converting store and forward flag
    private static string ConvertStoreAndFwdFlag(string value)
    {
        return value.Trim().ToUpper() switch
        {
            "N" => "No",
            "Y" => "Yes",
            _ => value.Trim()
        };
    }

    // Method for converting to UTC
    private static DateTime ConvertToUtc(DateTime estDateTime)
        => TimeZoneInfo.ConvertTimeToUtc(estDateTime, EstTimeZone);
}
