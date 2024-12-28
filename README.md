# CSV File Parser (and Data Importer)

This project implements a CSV parser and data importer for taxi trip data. It processes a CSV file, removes duplicates, and imports the data into a SQL Server database.

## Notes

- This program is designed to handle large CSV files efficiently, using streaming and bulk insert techniques.
- Duplicate records are removed and saved to a separate `duplicates.csv` file.
- The program converts 'Y' to 'Yes' and 'N' to 'No' for the `StoreAndFwdFlag` column.
- All text fields are trimmed of leading and trailing whitespaces.
- Datetime values are converted from EST to UTC before insertion into the database.

## SQL Scripts

The following SQL script was used to create the table in the database:

```sql
-- 'Trips' table creating
CREATE TABLE Trips (
    Id INT IDENTITY PRIMARY KEY,
    PickupDatetime DATETIME NOT NULL,
    DropoffDatetime DATETIME NOT NULL,
    PassengerCount INT,
    TripDistance FLOAT NOT NULL,
    StoreAndFwdFlag NVARCHAR(3) NOT NULL,
    PULocationID INT NOT NULL,
    DOLocationID INT NOT NULL,
    FareAmount DECIMAL(10, 2) NOT NULL,
    TipAmount DECIMAL(10, 2) NOT NULL
);

-- Index for 'PULocationID' (for search by landing place)
CREATE INDEX IX_Trips_PULocationID ON Trips (PULocationID);

-- Index for 'TripDistance' (to find the longest trips by distance)
CREATE INDEX IX_Trips_TripDistance ON Trips (TripDistance DESC);

-- Index for trip duration (to find the longest trips in time)
CREATE INDEX IX_Trips_PickupDropoff ON Trips (PickupDatetime, DropoffDatetime);

-- Index to find duplicates
CREATE UNIQUE INDEX IX_Trips_Unique ON Trips (PickupDatetime, DropoffDatetime, PassengerCount) 
WHERE PassengerCount IS NOT NULL;
```

## Number of rows

After running the program with the provided sample data, the number of rows in the `Trips` table is: **29889**.

**111** duplicates were removed and saved to `duplicates.csv`.
