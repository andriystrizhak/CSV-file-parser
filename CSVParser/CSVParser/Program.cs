using System;
using System.Linq;
using AutoMapper;
using CSVParser;
using CSVParser.Data;
using CSVParser.Mappers;
using CSVParser.Services;
using Microsoft.EntityFrameworkCore;

namespace CSVParserApp;

public class Program
{
    public static void Main(string[] args)
    {
        using (var context = new CSVParserDbContext())
        {
            bool continueRunning = true;
            while (continueRunning)
            {
                Console.WriteLine("\nSelect an option:");
                Console.WriteLine("1. Import data from CSV file.");
                Console.WriteLine("2. Get PULocationID with the highest average tip amount.");
                Console.WriteLine("3. Get top 100 longest trips by distance.");
                Console.WriteLine("4. Get top 100 longest trips by duration.");
                Console.WriteLine("5. Filter trips by PULocationID.");
                Console.WriteLine("q. Exit");
                Console.Write("Enter your choice: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ImportDataFromCsv(context);
                        break;
                    case "2":
                        GetPULocationWithHighestTip(context);
                        break;
                    case "3":
                        GetTop100LongestTripsByDistance(context);
                        break;
                    case "4":
                        GetTop100LongestTripsByDuration(context);
                        break;
                    case "5":
                        Console.Write("Enter PULocationID: ");
                        if (int.TryParse(Console.ReadLine(), out int puLocationId))
                            FilterTripsByPULocationID(context, puLocationId);
                        else
                            Console.WriteLine("Invalid input. Please enter a valid number.");
                        break;
                    case "q":
                        continueRunning = false;
                        Console.WriteLine("Exiting program...");
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }

    private static void ImportDataFromCsv(CSVParserDbContext context)
    {
        Console.Write("Enter the path to the CSV file: ");
        string? filePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            Console.WriteLine("Invalid file path or file does not exist.");
            return;
        }

        var csvReader = new CsvReaderService();
        var duplicateHandler = new DuplicateHandler();
        var tripService = new TripService(context);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<TripRecordToTripMap>());
        var mapper = config.CreateMapper();

        var records = csvReader.ReadCsv(filePath);
        var (uniqueRecords, duplicates) = duplicateHandler.ProcessRecords(records);

        var uniqueTrips = mapper.Map<IEnumerable<Trip>>(uniqueRecords);

        duplicateHandler.WriteDuplicatesToCsv(duplicates, "duplicates.csv");

        tripService.BulkInsertTripsAsync(uniqueTrips).Wait();

        Console.WriteLine($"Imported {uniqueRecords.Count()} unique records. {duplicates.Count()} duplicates were removed and saved to duplicates.csv");
    }

    private static void GetPULocationWithHighestTip(CSVParserDbContext context)
    {
        var result = context.Trips
            .GroupBy(t => t.PULocationID)
            .Select(g => new { PULocationID = g.Key, AvgTip = g.Average(t => t.TipAmount) })
            .OrderByDescending(x => x.AvgTip)
            .FirstOrDefault();

        if (result != null)
            Console.WriteLine($"PULocationID with the highest average tip: {result.PULocationID} (AvgTip: {result.AvgTip:C})");
        else
            Console.WriteLine("No data available.");
    }

    private static void GetTop100LongestTripsByDistance(CSVParserDbContext context)
    {
        var trips = context.Trips
            .OrderByDescending(t => t.TripDistance)
            .Take(100)
            .ToList();

        Console.WriteLine("Top 100 longest trips by distance:");
        foreach (var trip in trips)
            Console.WriteLine($"Trip ID: {trip.Id}, Distance: {trip.TripDistance:F2}");
    }

    private static void GetTop100LongestTripsByDuration(CSVParserDbContext context)
    {
        var trips = context.Trips
            .Select(t => new
            {
                t.Id,
                Duration = EF.Functions.DateDiffMinute(t.PickupDatetime, t.DropoffDatetime)
            })
            .OrderByDescending(t => t.Duration)
            .Take(100)
            .ToList();

        Console.WriteLine("Top 100 longest trips by duration:");
        foreach (var trip in trips)
            Console.WriteLine($"Trip ID: {trip.Id}, Duration: {trip.Duration} minutes");
    }

    private static void FilterTripsByPULocationID(CSVParserDbContext context, int pulocationId)
    {
        var trips = context.Trips
            .Where(t => t.PULocationID == pulocationId)
            .ToList();

        Console.WriteLine($"Trips with PULocationID {pulocationId}:");
        foreach (var trip in trips)
            Console.WriteLine($"Trip ID: {trip.Id}, Distance: {trip.TripDistance:F2} miles, Tip: {trip.TipAmount:C2}");
    }
}