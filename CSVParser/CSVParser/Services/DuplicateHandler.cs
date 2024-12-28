using CsvHelper;
using CSVParser.Models;

namespace CSVParser.Services;

public class DuplicateHandler
{
    public (IEnumerable<TripRecord> Unique, IEnumerable<TripRecord> Duplicates) ProcessRecords(IEnumerable<TripRecord> records)
    {
        var uniqueRecords = new Dictionary<(DateTime, DateTime, int?), TripRecord>();
        var duplicates = new List<TripRecord>();

        foreach (var record in records)
        {
            var key = (record.TpepPickupDatetime, record.TpepDropoffDatetime, record.PassengerCount);
            if (!uniqueRecords.ContainsKey(key))
            {
                uniqueRecords[key] = record;
            }
            else
            {
                duplicates.Add(record);
            }
        }

        return (uniqueRecords.Values, duplicates);
    }

    public void WriteDuplicatesToCsv(IEnumerable<TripRecord> duplicates, string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        using (var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(duplicates);
        }
    }
}
