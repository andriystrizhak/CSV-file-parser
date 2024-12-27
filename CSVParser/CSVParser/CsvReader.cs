using CsvHelper;
using CsvHelper.Configuration;
using CSVParser.Models;
using System.Globalization;

namespace CSVParser;

public class CsvReader
{
    public IEnumerable<TripRecord> ReadCsv(string filePath)
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            Delimiter = ",",
            Mode = CsvMode.RFC4180,
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvHelper.CsvReader(reader, configuration))
        {
            csv.Context.RegisterClassMap<CsvToTripRecordMap>();
            var records = csv.GetRecords<TripRecord>();
            foreach (var record in records)
            {
                yield return record;
            }
        }
    }
}
