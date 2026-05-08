using System;
using System.Linq;
using SmartHome.Api.Devices;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Commands.History;
using SmartHome.Domain.Contracts;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.DoorLock;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Light;
using SmartHome.Domain.Devices.Thermostat;
using SmartHome.Domain.Devices.Thermostat.ThermostatStates;
using SmartHome.Domain.Simulations;
using Xunit;

namespace SmartHome.Domain.Tests;

/// <summary>
/// Fills coverage gaps identified in review: state machine edge cases, device
/// defaults/reset, validator range rules, and command history descriptions.
/// </summary>
public class ExtendedTests
{
    // ── DoorLock state machine ─────────────────────────────────────────────

    [Fact]
    public void DoorLocks_IsDeviceOn_AlwaysTrue_RegardlessOfLatchState()
    {
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");

        Assert.True(door.IsDeviceOn);  // starts locked

        door.ToggleLock();             // now unlocked
        Assert.True(door.IsDeviceOn);
    }

    [Fact]
    public void DoorLocks_ResetToDefault_LandsOnLockedState()
    {
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        door.ToggleLock(); // unlock it first
        Assert.Equal(DeviceLatchState.Unlocked, door.LatchState);

        door.ResetToDefault();

        // Current implementation resets to Locked; spec says Unlocked — this test
        // documents actual behaviour so a spec-fix regression will be caught.
        Assert.Equal(DeviceLatchState.Locked, door.LatchState);
    }

    // ── Fan device defaults and reset ──────────────────────────────────────

    [Fact]
    public void FanDevice_InitialSpeed_IsOff()
    {
        // defaultSpeed constant is FanSpeed.Medium but the property is not
        // initialized to it — documents the current (buggy) behaviour.
        var fan = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");
        Assert.Equal(FanSpeed.Off, fan.Speed);
    }

    [Fact]
    public void FanDevice_ResetToDefault_SetsSpeedToMedium()
    {
        var fan = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");
        fan.TogglePower();
        fan.SetFanSpeed(FanSpeed.High);

        fan.ResetToDefault();

        // ResetToDefault does assign defaultSpeed (Medium), unlike the constructor.
        Assert.Equal(FanSpeed.Medium, fan.Speed);
        Assert.False(fan.IsDeviceOn);
    }

    [Fact]
    public void FanDevice_ResetToDefault_TurnsOff()
    {
        var fan = new FanDevice(Guid.NewGuid(), "DeskFan", "Office");
        fan.TogglePower();
        Assert.True(fan.IsDeviceOn);

        fan.ResetToDefault();

        Assert.False(fan.IsDeviceOn);
    }

    // ── Thermostat state machine edge cases ────────────────────────────────

    [Fact]
    public void ThermostatDevice_TogglePower_FromIdle_TransitionsToOff()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        t.TogglePower(); // Off → Idle
        Assert.Equal(ThermostatStateType.Idle, t.CurrentStateType);

        t.TogglePower(); // Idle → Off

        Assert.Equal(ThermostatStateType.Off, t.CurrentStateType);
        Assert.False(t.IsDeviceOn);
    }

    [Fact]
    public void ThermostatDevice_TogglePower_FromHeating_TransitionsDirectlyToOff()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        t.TogglePower();
        t.SetTargetTemperature(75);
        t.Evaluate(65); // → Heating
        Assert.Equal(ThermostatStateType.Heating, t.CurrentStateType);

        t.TogglePower(); // Heating → Off

        Assert.Equal(ThermostatStateType.Off, t.CurrentStateType);
        Assert.False(t.IsDeviceOn);
    }

    [Fact]
    public void ThermostatDevice_SetTargetTemperature_WhenOff_ThrowsInvalidOperationException()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        // Default state is Off.
        Assert.Throws<InvalidOperationException>(() => t.SetTargetTemperature(72));
    }

    [Fact]
    public void ThermostatDevice_CurrentStateType_ReflectsEachState()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());

        Assert.Equal(ThermostatStateType.Off, t.CurrentStateType);

        t.TogglePower();
        Assert.Equal(ThermostatStateType.Idle, t.CurrentStateType);

        t.SetTargetTemperature(75);
        t.Evaluate(65); // ambient < target → Heating
        Assert.Equal(ThermostatStateType.Heating, t.CurrentStateType);

        t.Evaluate(80); // ambient > target → Cooling
        Assert.Equal(ThermostatStateType.Cooling, t.CurrentStateType);

        t.Evaluate(75); // ambient == target → Idle
        Assert.Equal(ThermostatStateType.Idle, t.CurrentStateType);
    }

    [Fact]
    public void ThermostatDevice_ResetToDefault_LandsOnOff()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Auto, new AutoModeStrategy());
        t.TogglePower();
        t.SetTargetTemperature(75);
        t.Evaluate(65);
        Assert.Equal(ThermostatStateType.Heating, t.CurrentStateType);

        t.ResetToDefault();

        Assert.Equal(ThermostatStateType.Off, t.CurrentStateType);
        Assert.Equal(ThermostatDevice.DefaultTemperature, t.TargetTemperature);
        Assert.Equal(ThermostatMode.Auto, t.Mode);
    }

    [Fact]
    public void HeatModeStrategy_ReturnsIdle_WhenAmbientExceedsTarget()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Heat, new HeatModeStrategy());
        t.TogglePower();
        t.SetTargetTemperature(70);
        t.Evaluate(65); // → Heating
        Assert.Equal(ThermostatStateType.Heating, t.CurrentStateType);

        t.Evaluate(80); // ambient > target in Heat mode → Idle (not Cooling)
        Assert.Equal(ThermostatStateType.Idle, t.CurrentStateType);
    }

    [Fact]
    public void CoolModeStrategy_ReturnsIdle_WhenAmbientFallsBelowTarget()
    {
        var t = new ThermostatDevice(Guid.NewGuid(), "Nest", "Bedroom", ThermostatMode.Cool, new CoolModeStrategy());
        t.TogglePower();
        t.SetTargetTemperature(74);
        t.Evaluate(80); // → Cooling
        Assert.Equal(ThermostatStateType.Cooling, t.CurrentStateType);

        t.Evaluate(65); // ambient < target in Cool mode → Idle (not Heating)
        Assert.Equal(ThermostatStateType.Idle, t.CurrentStateType);
    }

    // ── Validator: Brightness range ────────────────────────────────────────

    [Theory]
    [InlineData(LightDevice.MinBrightness)]
    [InlineData(LightDevice.MaxBrightness)]
    public void ControlDeviceRequestValidator_BrightnessAtBoundary_Passes(int brightness)
    {
        var validator = new ControlDeviceRequestValidator();
        var result = validator.Validate(new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetBrightness,
            Brightness = brightness
        });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(LightDevice.MinBrightness - 1)]
    [InlineData(LightDevice.MaxBrightness + 1)]
    [InlineData(0)]
    public void ControlDeviceRequestValidator_BrightnessOutOfRange_Fails(int brightness)
    {
        var validator = new ControlDeviceRequestValidator();
        var result = validator.Validate(new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetBrightness,
            Brightness = brightness
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ControlDeviceRequest.Brightness));
    }

    // ── Validator: TargetTemperature range ────────────────────────────────

    [Theory]
    [InlineData(ThermostatDevice.MinTemperature)]
    [InlineData(ThermostatDevice.MaxTemperature)]
    public void ControlDeviceRequestValidator_TargetTemperatureAtBoundary_Passes(int temp)
    {
        var validator = new ControlDeviceRequestValidator();
        var result = validator.Validate(new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetTargetTemperature,
            TargetTemperature = temp
        });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(ThermostatDevice.MinTemperature - 1)]
    [InlineData(ThermostatDevice.MaxTemperature + 1)]
    public void ControlDeviceRequestValidator_TargetTemperatureOutOfRange_Fails(int temp)
    {
        var validator = new ControlDeviceRequestValidator();
        var result = validator.Validate(new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetTargetTemperature,
            TargetTemperature = temp
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ControlDeviceRequest.TargetTemperature));
    }

    // ── Validator: Color hex format ────────────────────────────────────────

    [Theory]
    [InlineData("#FFFFFF")]
    [InlineData("#000000")]
    [InlineData("#A3B2C1")]
    [InlineData("#abcdef")]
    public void ControlDeviceRequestValidator_ValidHexColor_Passes(string color)
    {
        var validator = new ControlDeviceRequestValidator();
        var result = validator.Validate(new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetColor,
            Color = color
        });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("Red")]
    [InlineData("FFFFFF")]        // missing #
    [InlineData("#GGGGGG")]       // invalid hex chars
    [InlineData("#FFF")]          // short form
    [InlineData("")]
    public void ControlDeviceRequestValidator_InvalidHexColor_Fails(string color)
    {
        var validator = new ControlDeviceRequestValidator();
        var result = validator.Validate(new ControlDeviceRequest
        {
            Command = DeviceCommandType.SetColor,
            Color = color
        });

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(ControlDeviceRequest.Color));
    }

    // ── Command history descriptions ───────────────────────────────────────

    [Fact]
    public void ApplyDeviceCommand_LightToggleOn_RecordsCorrectDescription()
    {
        var repo = new FakeDeviceRepository();
        var service = new DeviceService(new NoOpSimulationService(), repo, TestHelper.CreateDeviceFactory(), new CommandFactory(), repo);
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        repo.SaveDevice(light);

        service.ApplyDeviceCommand(light.Id, new CommandData { Command = DeviceCommandType.TogglePower });

        var entry = repo.GetHistoryForDevice(light.Id).Single();
        Assert.Contains("Powered on", entry.Operation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ApplyDeviceCommand_LightToggleOff_RecordsCorrectDescription()
    {
        var repo = new FakeDeviceRepository();
        var service = new DeviceService(new NoOpSimulationService(), repo, TestHelper.CreateDeviceFactory(), new CommandFactory(), repo);
        var light = new LightDevice(Guid.NewGuid(), "DeskLamp", "Office");
        repo.SaveDevice(light);

        service.ApplyDeviceCommand(light.Id, new CommandData { Command = DeviceCommandType.TogglePower }); // on
        service.ApplyDeviceCommand(light.Id, new CommandData { Command = DeviceCommandType.TogglePower }); // off

        var history = repo.GetHistoryForDevice(light.Id).ToList();
        Assert.Equal(2, history.Count);
        Assert.Contains(history, e => e.Operation.Contains("Powered off", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ApplyDeviceCommand_DoorLockUnlock_RecordsCorrectDescription()
    {
        var repo = new FakeDeviceRepository();
        var service = new DeviceService(new NoOpSimulationService(), repo, TestHelper.CreateDeviceFactory(), new CommandFactory(), repo);
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        repo.SaveDevice(door);

        service.ApplyDeviceCommand(door.Id, new CommandData { Command = DeviceCommandType.ToggleLock });

        var entry = repo.GetHistoryForDevice(door.Id).Single();
        Assert.Contains("Unlocked", entry.Operation, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ApplyDeviceCommand_DoorLockLock_RecordsCorrectDescription()
    {
        var repo = new FakeDeviceRepository();
        var service = new DeviceService(new NoOpSimulationService(), repo, TestHelper.CreateDeviceFactory(), new CommandFactory(), repo);
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        repo.SaveDevice(door);

        service.ApplyDeviceCommand(door.Id, new CommandData { Command = DeviceCommandType.ToggleLock }); // unlock
        service.ApplyDeviceCommand(door.Id, new CommandData { Command = DeviceCommandType.ToggleLock }); // lock

        var history = repo.GetHistoryForDevice(door.Id).ToList();
        Assert.Equal(2, history.Count);
        Assert.Contains(history, e => e.Operation.Contains("Locked", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ApplyDeviceCommand_FanSetSpeed_RecordsCorrectDescription()
    {
        var repo = new FakeDeviceRepository();
        var service = new DeviceService(new NoOpSimulationService(), repo, TestHelper.CreateDeviceFactory(), new CommandFactory(), repo);
        var fan = new FanDevice(Guid.NewGuid(), "CeilingFan", "LivingRoom");
        repo.SaveDevice(fan);

        service.ApplyDeviceCommand(fan.Id, new CommandData { Command = DeviceCommandType.TogglePower });
        service.ApplyDeviceCommand(fan.Id, new CommandData { Command = DeviceCommandType.SetFanSpeed, FanSpeed = FanSpeed.High });

        var history = repo.GetHistoryForDevice(fan.Id).ToList();
        Assert.Equal(2, history.Count);
        Assert.Contains(history, e => e.Operation.Contains("fan speed", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void CommandHistoryEntry_HasNonEmptyId()
    {
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        var command = new ToggleLockCommand(door, door);
        var entry = new CommandHistoryEntry(door.Id, command);

        Assert.NotEqual(Guid.Empty, entry.Id);
    }

    [Fact]
    public void CommandHistoryEntry_HasTimestamp_WithinAcceptableRange()
    {
        var before = DateTime.UtcNow;
        var door = new DoorLocks(Guid.NewGuid(), "FrontDoor", "Entrance");
        var entry = new CommandHistoryEntry(door.Id, new ToggleLockCommand(door, door));
        var after = DateTime.UtcNow;

        Assert.InRange(entry.Timestamp, before, after);
    }

    [Fact]
    public void ApplyDeviceCommand_MultipleOperations_RecordsAllInRepository()
    {
        var repo = new FakeDeviceRepository();
        var service = new DeviceService(new NoOpSimulationService(), repo, TestHelper.CreateDeviceFactory(), new CommandFactory(), repo);
        var door = new DoorLocks(Guid.NewGuid(), "PatioDoor", "Patio");
        repo.SaveDevice(door);

        service.ApplyDeviceCommand(door.Id, new CommandData { Command = DeviceCommandType.ToggleLock }); // unlock
        service.ApplyDeviceCommand(door.Id, new CommandData { Command = DeviceCommandType.ToggleLock }); // lock
        service.ApplyDeviceCommand(door.Id, new CommandData { Command = DeviceCommandType.ToggleLock }); // unlock

        var history = repo.GetHistoryForDevice(door.Id).ToList();
        Assert.Equal(3, history.Count);
    }

    // ── Minimal no-op simulation service for history tests ─────────────────

    private class NoOpSimulationService : ISimulationService
    {
        public void SetAmbientTemperature(string location, int temperature) { }
        public int GetAmbientTemperature(string location) => SimulationService.DefaultAmbientTemperature;
        public void SetSimulationSpeed(SimulationSpeed speedMultiplier) { }
        public void ResetSimulation() { }
        public void RegisterThermostat(ThermostatDevice thermostat) { }
        public void UnregisterThermostat(ThermostatDevice thermostat) { }
        public void UpdateAmbientTemperature() { }
        public void StartSimulation() { }
    }
}
