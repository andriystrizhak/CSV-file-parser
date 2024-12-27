using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CSVParser.Data;

public partial class CSVParserDbContext : DbContext
{
    public CSVParserDbContext()
    {
    }

    public CSVParserDbContext(DbContextOptions<CSVParserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Trip> Trips { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Trip>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Trips__3214EC07160D6911");

            entity.HasIndex(e => e.PULocationID, "IX_Trips_PULocationID");

            entity.HasIndex(e => new { e.PickupDatetime, e.DropoffDatetime }, "IX_Trips_PickupDropoff");

            entity.HasIndex(e => e.TripDistance, "IX_Trips_TripDistance").IsDescending();

            entity.HasIndex(e => new { e.PickupDatetime, e.DropoffDatetime, e.PassengerCount }, "IX_Trips_Unique")
                .IsUnique()
                .HasFilter("([PassengerCount] IS NOT NULL)");

            entity.Property(e => e.DOLocationID).HasColumnName("DOLocationID");
            entity.Property(e => e.DropoffDatetime).HasColumnType("datetime");
            entity.Property(e => e.FareAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PickupDatetime).HasColumnType("datetime");
            entity.Property(e => e.PULocationID).HasColumnName("PULocationID");
            entity.Property(e => e.StoreAndFwdFlag).HasMaxLength(3);
            entity.Property(e => e.TipAmount).HasColumnType("decimal(10, 2)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
