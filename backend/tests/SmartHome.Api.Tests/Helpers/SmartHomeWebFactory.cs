using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Locations;
using SmartHome.Infrastructure;
using SmartHome.Infrastructure.ORM_Persistence;

namespace SmartHome.Api.Tests.Helpers;

/// <summary>
/// WebApplicationFactory for integration tests.
/// Replaces the production SQLite file and the legacy JsonRepository with an
/// in-memory SQLite database that is created fresh for each test class.
/// </summary>
public class SmartHomeWebFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public SmartHomeWebFactory()
    {
        // Named in-memory database with shared cache keeps the DB alive
        // across multiple connections opened by the same test run.
        var name = Guid.NewGuid().ToString("N");
        _connection = new SqliteConnection($"DataSource=file:{name}?mode=memory&cache=shared");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ── Remove production persistence registrations ────────────────
            RemoveAll<DbContextOptions<SmartHomeDbContext>>(services);
            RemoveAll<SmartHomeDbContext>(services);
            RemoveAll<JsonRepository>(services);
            RemoveAll<IDeviceRepository>(services);
            RemoveAll<ILocationRepository>(services);

            // ── Register a shared Singleton DbContext (in-memory SQLite) ──
            // Singleton lifetime matches the production JsonRepository lifetime
            // so that singletons such as SimulationInitializer can safely
            // capture IDeviceRepository without a captive-dependency error.
            var options = new DbContextOptionsBuilder<SmartHomeDbContext>()
                .UseSqlite(_connection)
                .Options;
            var dbContext = new SmartHomeDbContext(options);
            services.AddSingleton(dbContext);

            // ── Re-register repositories as Singleton ─────────────────────
            services.AddSingleton(sp => new SqliteRepository(
                sp.GetRequiredService<SmartHomeDbContext>(),
                sp.GetRequiredService<IDeviceFactory>()));

            services.AddSingleton<IDeviceRepository>(
                sp => sp.GetRequiredService<SqliteRepository>());

            services.AddSingleton<ILocationRepository>(
                sp => sp.GetRequiredService<SqliteRepository>());
        });
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private static void RemoveAll<T>(IServiceCollection services)
    {
        var toRemove = services.Where(d => d.ServiceType == typeof(T)).ToList();
        foreach (var d in toRemove) services.Remove(d);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _connection.Dispose();
        base.Dispose(disposing);
    }
}
