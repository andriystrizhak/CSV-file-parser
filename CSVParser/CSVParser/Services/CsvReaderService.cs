using CsvHelper;
using CsvHelper.Configuration;
using CSVParser.Mappers;
using CSVParser.Models;
using System.Globalization;

namespace CSVParser.Services;

public class CsvReaderService
{
    public IEnumerable<TripRecord> ReadCsv(string filePath)
    {
        var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            Delimiter = ",",
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, configuration))
        {
            csv.Context.RegisterClassMap<CsvToTripRecordMap>();
            foreach (var record in csv.GetRecords<TripRecord>())
            {
                yield return record;
            }
        }
    }
}
