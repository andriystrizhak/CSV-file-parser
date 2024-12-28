using CSVParser.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;

namespace CSVParser.Services;

public class TripService(CSVParserDbContext context)
{
    public async Task<List<Trip>> GetAllTripsAsync() 
        => await context.Trips.ToListAsync();

    public async Task<Trip?> GetTripByIdAsync(int id)
        => await context.Trips.FindAsync(id);

    public async Task AddTripAsync(Trip trip)
    {
        await context.Trips.AddAsync(trip);
        await context.SaveChangesAsync();
    }

    public async Task AddTripsAsync(IEnumerable<Trip> trips)
    {
        await context.Trips.AddRangeAsync(trips);
        await context.SaveChangesAsync();
    }

    public async Task<List<Trip>> GetTop100LongestTripsByDistanceAsync()
        => await context.Trips
            .OrderByDescending(t => t.TripDistance)
            .Take(100)
            .ToListAsync();

    public async Task<List<Trip>> GetTop100LongestTripsByDurationAsync()
        => await context.Trips
            .OrderByDescending(t => EF.Functions.DateDiffSecond(t.PickupDatetime, t.DropoffDatetime))
            .Take(100)
            .ToListAsync();

    public async Task<double> GetAverageTipAmountByPULocationIdAsync(int puLocationId)
        => await context.Trips
            .Where(t => t.PULocationID == puLocationId)
            .AverageAsync(t => (double)t.TipAmount);

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
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("PickupDatetime", typeof(DateTime));
        dataTable.Columns.Add("DropoffDatetime", typeof(DateTime));
        dataTable.Columns.Add("PassengerCount", typeof(int));
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
            dataTable.Rows.Add(
                DBNull.Value,
                trip.PickupDatetime,
                trip.DropoffDatetime, 
                trip.PassengerCount.HasValue ? trip.PassengerCount.Value : (object)DBNull.Value,
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
        var connectionString = context.Database.GetConnectionString();
        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            using (var bulkCopy = new SqlBulkCopy(connection))
            {
                bulkCopy.DestinationTableName = "Trips";
                bulkCopy.BatchSize = 1000;

                for (int i = 0; i < dataTable.Columns.Count; i++)
                    bulkCopy.ColumnMappings.Add(dataTable.Columns[i].ColumnName, dataTable.Columns[i].ColumnName);

                await bulkCopy.WriteToServerAsync(dataTable);
            }
        }
    }
}
