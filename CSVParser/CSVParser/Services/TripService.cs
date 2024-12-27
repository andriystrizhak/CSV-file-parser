using CSVParser.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CSVParser.Services;

public class TripService
{
    private readonly CSVParserDbContext _context;

    public TripService(CSVParserDbContext context)
    {
        _context = context;
    }

    public async Task<List<Trip>> GetAllTripsAsync()
    {
        return await _context.Trips.ToListAsync();
    }

    public async Task<Trip?> GetTripByIdAsync(int id)
    {
        return await _context.Trips.FindAsync(id);
    }

    public async Task AddTripAsync(Trip trip)
    {
        await _context.Trips.AddAsync(trip);
        await _context.SaveChangesAsync();
    }

    public async Task AddTripsAsync(IEnumerable<Trip> trips)
    {
        await _context.Trips.AddRangeAsync(trips);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Trip>> GetTop100LongestTripsByDistanceAsync()
    {
        return await _context.Trips
            .OrderByDescending(t => t.TripDistance)
            .Take(100)
            .ToListAsync();
    }

    public async Task<List<Trip>> GetTop100LongestTripsByDurationAsync()
    {
        return await _context.Trips
            .OrderByDescending(t => EF.Functions.DateDiffSecond(t.PickupDatetime, t.DropoffDatetime))
            .Take(100)
            .ToListAsync();
    }

    public async Task<double> GetAverageTipAmountByPULocationIdAsync(int puLocationId)
    {
        return await _context.Trips
            .Where(t => t.PULocationID == puLocationId)
            .AverageAsync(t => (double)t.TipAmount);
    }

    public async Task BulkInsertTripsAsync(IEnumerable<Trip> trips)
    {
        var dataTable = CreateDataTable(trips);
        await BulkInsertDataTableAsync(dataTable);
    }

    private DataTable CreateDataTable(IEnumerable<Trip> trips)
    {
        var dataTable = new DataTable();
        SetupDataTableColumns(dataTable);
        PopulateDataTable(dataTable, trips);
        return dataTable;
    }

    private void SetupDataTableColumns(DataTable dataTable)
    {
        dataTable.Columns.Add("PickupDatetime", typeof(DateTime));
        dataTable.Columns.Add("DropoffDatetime", typeof(DateTime));
        dataTable.Columns.Add("PassengerCount", typeof(int?));
        dataTable.Columns.Add("TripDistance", typeof(float));
        dataTable.Columns.Add("StoreAndFwdFlag", typeof(string));
        dataTable.Columns.Add("PULocationID", typeof(int));
        dataTable.Columns.Add("DOLocationID", typeof(int));
        dataTable.Columns.Add("FareAmount", typeof(decimal));
        dataTable.Columns.Add("TipAmount", typeof(decimal));
    }

    private void PopulateDataTable(DataTable dataTable, IEnumerable<Trip> trips)
    {
        foreach (var trip in trips)
        {
            if (trip.PickupDatetime == default || trip.DropoffDatetime == default)
            {
                Console.WriteLine($"Warning: Invalid datetime for trip. PickupDatetime: {trip.PickupDatetime}, DropoffDatetime: {trip.DropoffDatetime}");
                continue; // Skip this record
            }

            dataTable.Rows.Add(
                trip.PickupDatetime,
                trip.DropoffDatetime,
                trip.PassengerCount.HasValue ? (object)trip.PassengerCount.Value : DBNull.Value, trip.TripDistance,
                trip.TripDistance,
                trip.StoreAndFwdFlag,
                trip.PULocationID,
                trip.DOLocationID,
                trip.FareAmount,
                trip.TipAmount
            );
        }
    }

    private async Task BulkInsertDataTableAsync(DataTable dataTable)
    {
        var connectionString = _context.Database.GetConnectionString();
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Trips";
                bulkCopy.BatchSize = 10000;

                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }
    }
}
