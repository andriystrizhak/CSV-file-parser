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

## How to Run the Program

Follow these steps to set up and run the CSV Parser and Data Importer:

1. **Prerequisites**
   - .NET 8.0 SDK
   - SQL Server (2022)

2. **Clone the Repository**
   - Clone this repository to your local machine

3. **Install Required Packages**
   - Open a terminal in the project root directory
   - Run the following command to restore all necessary NuGet packages:
     ```
     dotnet restore
     ```
     
3. **Configuration**
   - Open the `appsettings.json` file in the project folder
   - Update the `ConnectionStrings:DefaultConnection` with your SQL Server connection string:
     ```json
     "ConnectionStrings": {
       "DefaultConnection": "Server=your_server;Database=your_database;User Id=your_username;Password=your_password;"
     }
     ```

4. **Create Database from Model**
   - In the terminal, run the following commands to create the database and apply migrations:
     ```
     dotnet ef database update
     ```
   - If you haven't created migrations yet, run these commands first:
     ```
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

5. **Build the Project**
   - Open a terminal in the project root directory
   - Run the following command to build the project:
     ```
     dotnet build
     ```

6. **Run the Program**
   - In the terminal, run the following command:
     ```
     dotnet run
     ```
   - The program will start and display a menu of options

7. **Import Data**
   - Choose option 1 from the menu to import data from a CSV file
   - When prompted, enter the full path to your CSV file
   - The program will process the file, remove duplicates, and import the data into the database

8. **View Results**
   - After the import is complete, you can use the other menu options to view various statistics and query the data
   - To see the number of imported rows, you can run a SQL query in your database management tool:
     ```sql
     SELECT COUNT(*) FROM Trips;
     ```

9. **Exit the Program**
   - Choose option 'q' from the menu to exit the program

Note: Ensure that you have sufficient disk space for processing large CSV files and for storing the database. 
The program creates a `duplicates.csv` file in the same directory as the executable, so make sure you have write permissions in that location.
