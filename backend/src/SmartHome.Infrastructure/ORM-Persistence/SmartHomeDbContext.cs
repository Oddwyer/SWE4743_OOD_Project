using Microsoft.EntityFrameworkCore;

namespace SmartHome.Infrastructure.ORM_Persistence
{
    public class SmartHomeDbContext : DbContext
{
    public SmartHomeDbContext(DbContextOptions<SmartHomeDbContext> options) 
    : base(options) {}

    public DbSet<DeviceEntity> Devices { get; set; } // table created for our device entities
    public DbSet<LocationEntity> Locations { get; set; } // table created for our location entities
    public DbSet<CommandHistoryEntity> CommandHistories { get; set; } // table created for our command history entities

// general configurations for all entities and tables will happen below in the OnModelCreating method, 
// such as setting primary keys, required fields, and relationships between tables
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure DeviceEntity
        modelBuilder.Entity<DeviceEntity>(entity =>
        {
            entity.HasKey(e => e.Id); // Id is the primary key
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Location).IsRequired();
            entity.Property(e => e.Type).IsRequired();
        });

        // Configure LocationEntity
        modelBuilder.Entity<LocationEntity>(entity =>
        {
            entity.HasKey(e => e.Location); // Use Location as the primary key
            entity.Property(e => e.AmbientTemperature).IsRequired();
        });

        // Configure CommandHistoryEntity
        modelBuilder.Entity<CommandHistoryEntity>(entity =>
        {
            entity.HasKey(e => e.Id); // Id is the primary key
            entity.Property(e => e.DeviceId).IsRequired();
            entity.Property(e => e.CommandExecuted).IsRequired();
            entity.Property(e => e.Timestamp).IsRequired();

            // Set up foreign key relationship with DeviceEntity
            entity.HasOne<DeviceEntity>()
                  .WithMany()
                  .HasForeignKey(ch => ch.DeviceId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
}
