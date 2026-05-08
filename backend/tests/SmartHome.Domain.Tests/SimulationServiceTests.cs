using System;
using System.Collections.Generic;
using System.Linq;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using SmartHome.Domain.Locations;
using SmartHome.Domain.Simulations;
using Xunit;

namespace SmartHome.Domain.Tests;

public class SimulationServiceTests
{
    // Returns null for missing keys so RegisterThermostat's null-check works correctly.
    private class SimFakeRepo : IDeviceRepository, ILocationRepository
    {
        private readonly Dictionary<Guid, IDevice> _devices = new();
        private readonly Dictionary<string, int> _temps = new();

        public IEnumerable<IDevice> FindAllDevices(DeviceFilter filter) => _devices.Values;
        public IDevice? FindDeviceById(Guid id) => _devices.TryGetValue(id, out var d) ? d : null;
        public IDevice SaveDevice(IDevice device) { _devices[device.Id] = device; return device; }
        public void DeleteDevice(Guid id) => _devices.Remove(id);
        public bool ThermostatInLocation(string loc) => false;
        public IEnumerable<CommandHistoryEntry> GetHistoryForDevice(Guid id) => Enumerable.Empty<CommandHistoryEntry>();
        public void SaveHistoryEntry(CommandHistoryEntry entry) { }

        public int? GetAmbientTemperature(string location) =>
            _temps.TryGetValue(location, out var t) ? t : null;

        public void SaveAmbientTemperature(string location, int temperature) =>
            _temps[location] = temperature;
    }

    private static (SimulationService service, SimFakeRepo repo) CreateService()
    {
        var repo = new SimFakeRepo();
        var ticker = new SimulationTicker();
        var runtime = new SimulationRuntime(ticker);
        var service = new SimulationService(repo, repo, runtime);
        return (service, repo);
    }

    private static ThermostatDevice CreateThermostat(string location = "Bedroom") =>
        new ThermostatDevice(Guid.NewGuid(), "Nest", location, ThermostatMode.Auto, new AutoModeStrategy());

    // ── SetAmbientTemperature ──────────────────────────────────────────────

    [Fact]
    public void SetAmbientTemperature_ThrowsArgumentException_WhenLocationIsEmpty()
    {
        var (service, _) = CreateService();
        Assert.Throws<ArgumentException>(() => service.SetAmbientTemperature("", 72));
    }

    [Fact]
    public void SetAmbientTemperature_ThrowsArgumentException_WhenLocationIsWhitespace()
    {
        var (service, _) = CreateService();
        Assert.Throws<ArgumentException>(() => service.SetAmbientTemperature("   ", 72));
    }

    [Theory]
    [InlineData(SimulationService.MinAmbientTemperature - 1)]
    [InlineData(-10)]
    public void SetAmbientTemperature_ThrowsArgumentOutOfRangeException_WhenBelowMinimum(int temp)
    {
        var (service, _) = CreateService();
        Assert.Throws<ArgumentOutOfRangeException>(() => service.SetAmbientTemperature("Bedroom", temp));
    }

    [Theory]
    [InlineData(SimulationService.MaxAmbientTemperature + 1)]
    [InlineData(200)]
    public void SetAmbientTemperature_ThrowsArgumentOutOfRangeException_WhenAboveMaximum(int temp)
    {
        var (service, _) = CreateService();
        Assert.Throws<ArgumentOutOfRangeException>(() => service.SetAmbientTemperature("Bedroom", temp));
    }

    [Theory]
    [InlineData(SimulationService.MinAmbientTemperature)]
    [InlineData(SimulationService.MaxAmbientTemperature)]
    public void SetAmbientTemperature_AcceptsBoundaryValues(int temp)
    {
        var (service, _) = CreateService();
        service.SetAmbientTemperature("Bedroom", temp);
        Assert.Equal(temp, service.GetAmbientTemperature("Bedroom"));
    }

    [Fact]
    public void SetAmbientTemperature_NormalizesLocationCasing()
    {
        var (service, _) = CreateService();
        service.SetAmbientTemperature("BEDROOM", 75);
        Assert.Equal(75, service.GetAmbientTemperature("bedroom"));
    }

    // ── GetAmbientTemperature ──────────────────────────────────────────────

    [Fact]
    public void GetAmbientTemperature_ReturnsDefault_WhenNoneStored()
    {
        var (service, _) = CreateService();
        var temp = service.GetAmbientTemperature("UnknownRoom");
        Assert.Equal(SimulationService.DefaultAmbientTemperature, temp);
    }

    [Fact]
    public void GetAmbientTemperature_ThrowsArgumentException_WhenLocationIsEmpty()
    {
        var (service, _) = CreateService();
        Assert.Throws<ArgumentException>(() => service.GetAmbientTemperature(""));
    }

    [Fact]
    public void GetAmbientTemperature_ReturnsStoredTemperature()
    {
        var (service, _) = CreateService();
        service.SetAmbientTemperature("Kitchen", 68);
        Assert.Equal(68, service.GetAmbientTemperature("Kitchen"));
    }

    // ── RegisterThermostat ─────────────────────────────────────────────────

    [Fact]
    public void RegisterThermostat_SetsDefaultAmbientTemperature_WhenNoneExists()
    {
        var (service, _) = CreateService();
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);

        Assert.Equal(SimulationService.DefaultAmbientTemperature, service.GetAmbientTemperature("Bedroom"));
    }

    [Fact]
    public void RegisterThermostat_DoesNotOverwriteExistingAmbientTemperature()
    {
        var (service, _) = CreateService();
        service.SetAmbientTemperature("Bedroom", 65);
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);

        Assert.Equal(65, service.GetAmbientTemperature("Bedroom"));
    }

    // ── UpdateAmbientTemperature ───────────────────────────────────────────

    [Fact]
    public void UpdateAmbientTemperature_IncrementsTemperature_WhenThermostatIsHeating()
    {
        var (service, _) = CreateService();
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);
        service.SetAmbientTemperature("Bedroom", 65);

        // TogglePower → Idle, then Evaluate puts it into Heating (ambient 65 < target 72).
        thermostat.TogglePower();
        thermostat.Evaluate(65);
        Assert.Equal(ThermostatStateType.Heating, thermostat.CurrentStateType);

        service.UpdateAmbientTemperature();

        Assert.Equal(66, service.GetAmbientTemperature("Bedroom"));
    }

    [Fact]
    public void UpdateAmbientTemperature_DecrementsTemperature_WhenThermostatIsCooling()
    {
        var (service, _) = CreateService();
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);
        service.SetAmbientTemperature("Bedroom", 80);

        thermostat.TogglePower();
        thermostat.Evaluate(80);
        Assert.Equal(ThermostatStateType.Cooling, thermostat.CurrentStateType);

        service.UpdateAmbientTemperature();

        Assert.Equal(79, service.GetAmbientTemperature("Bedroom"));
    }

    [Fact]
    public void UpdateAmbientTemperature_SkipsThermostat_WhenOff()
    {
        var (service, _) = CreateService();
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);
        service.SetAmbientTemperature("Bedroom", 65);
        // Thermostat left in Off state — no TogglePower call.

        service.UpdateAmbientTemperature();

        Assert.Equal(65, service.GetAmbientTemperature("Bedroom"));
    }

    [Fact]
    public void UpdateAmbientTemperature_DoesNotChange_WhenIdleAtTarget()
    {
        var (service, _) = CreateService();
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);
        // Default target is 72; set ambient to 72 so Idle.
        service.SetAmbientTemperature("Bedroom", 72);

        thermostat.TogglePower();
        thermostat.Evaluate(72);
        Assert.Equal(ThermostatStateType.Idle, thermostat.CurrentStateType);

        service.UpdateAmbientTemperature();

        Assert.Equal(72, service.GetAmbientTemperature("Bedroom"));
    }

    [Fact]
    public void SetAmbientTemperature_TriggersEvaluate_OnRegisteredHeatingThermostat()
    {
        var (service, _) = CreateService();
        var thermostat = CreateThermostat("Bedroom");

        service.RegisterThermostat(thermostat);
        service.SetAmbientTemperature("Bedroom", 65);

        thermostat.TogglePower();
        thermostat.Evaluate(65);
        Assert.Equal(ThermostatStateType.Heating, thermostat.CurrentStateType);

        // Ambient at target → thermostat should transition to Idle after SetAmbientTemperature.
        service.SetAmbientTemperature("Bedroom", 72);

        Assert.Equal(ThermostatStateType.Idle, thermostat.CurrentStateType);
    }
}
