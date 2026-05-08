using Microsoft.EntityFrameworkCore;

namespace SmartHome.Infrastructure.ORM_Persistence
{
    /// <summary>
    /// EF Core database context for the SmartHome application; manages devices, locations, and command history.
    /// </summary>
    public class SmartHomeDbContext : DbContext
    {
        /// <summary>Initializes the context with the provided EF Core options.</summary>
        public SmartHomeDbContext(DbContextOptions<SmartHomeDbContext> options)
            : base(options) {}

        /// <summary>Persisted device records.</summary>
        public DbSet<DeviceEntity> Devices { get; set; }
        /// <summary>Persisted location ambient temperature records.</summary>
        public DbSet<LocationEntity> Locations { get; set; }
        /// <summary>Persisted command history records.</summary>
        public DbSet<CommandHistoryEntity> CommandHistories { get; set; }

        /// <summary>Configures table schema, primary keys, required fields, and foreign key relationships.</summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DeviceEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Type).IsRequired();
            });

            modelBuilder.Entity<LocationEntity>(entity =>
            {
                entity.HasKey(e => e.Location);
                entity.Property(e => e.AmbientTemperature).IsRequired();
            });

            modelBuilder.Entity<CommandHistoryEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceId).IsRequired();
                entity.Property(e => e.CommandExecuted).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();

                entity.HasOne<DeviceEntity>()
                      .WithMany()
                      .HasForeignKey(ch => ch.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
