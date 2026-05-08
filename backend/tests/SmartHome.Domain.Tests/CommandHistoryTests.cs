using SmartHome.Domain.Commands;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Simulations;
using Xunit;

namespace SmartHome.Domain.Tests;

public class CommandHistoryTests
{
    [Fact]
    public void ApplyDeviceCommand_SavesHistoryEntry()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new CommandFactory();
        var service = new DeviceService(new FakeSimulationService(), repository, TestHelper.CreateDeviceFactory(), commandFactory, repository);

        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        repository.SaveDevice(light);

        service.ApplyDeviceCommand(light.Id, new CommandData { Command = DeviceCommandType.TogglePower });

        var history = repository.GetHistoryForDevice(light.Id).ToList();

        Assert.Single(history);
        Assert.Equal(light.Id, history[0].DeviceId);
        Assert.Contains("Powered on", history[0].Operation);
        Assert.True(history[0].Timestamp <= DateTime.UtcNow);
    }

    [Fact]
    public void GetCommandHistory_ReturnsHistoryEntriesForDevice()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new CommandFactory();
        var service = new DeviceService(new FakeSimulationService(), repository, TestHelper.CreateDeviceFactory(), commandFactory, repository);

        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        repository.SaveDevice(fan);
        service.ApplyDeviceCommand(fan.Id, new CommandData { Command = DeviceCommandType.TogglePower });
        service.ApplyDeviceCommand(fan.Id, new CommandData { Command = DeviceCommandType.SetFanSpeed, FanSpeed = FanSpeed.High });

        var history = service.GetCommandHistory(fan.Id).ToList();

        Assert.Equal(2, history.Count);
        Assert.Contains(history, entry => entry.Operation.Contains("Powered on"));
        Assert.Contains(history, entry => entry.Operation.Contains("Set fan speed"));
    }

    [Fact]
    public void ApplyDeviceCommand_ToMissingDevice_ThrowsKeyNotFoundException()
    {
        var repository = new FakeDeviceRepository();
        var commandFactory = new CommandFactory();
        var service = new DeviceService(new FakeSimulationService(), repository, TestHelper.CreateDeviceFactory(), commandFactory, repository);

        Assert.Throws<KeyNotFoundException>(() => service.ApplyDeviceCommand(Guid.NewGuid(), new CommandData { Command = DeviceCommandType.TogglePower }));
    }

    private class FakeSimulationService : ISimulationService
    {
        public void SetAmbientTemperature(string location, int temperature) { }
        public int GetAmbientTemperature(string location) => 70;
        public void SetSimulationSpeed(SimulationSpeed speedMultiplier) { }
        public void ResetSimulation() { }
        public void RegisterThermostat(ThermostatDevice thermostat) { }
        public void UnregisterThermostat(ThermostatDevice thermostat) { }
        public void UpdateAmbientTemperature() { }
        public void StartSimulation() { }

    }
}
