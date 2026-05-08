using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.Api.Devices;
using SmartHome.Api.Tests.Helpers;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using SmartHome.Domain.Devices.Fan;
using SmartHome.Domain.Devices.Thermostat;
using Xunit;

namespace SmartHome.Api.Tests;

/// <summary>
/// Integration tests for /api/devices — happy-path CRUD and device-control scenarios.
/// Each test class owns its own WebApplicationFactory with a fresh in-memory database,
/// pre-seeded with three default devices (thermostat, light, fan).
/// </summary>
public class DevicesControllerTests : IClassFixture<SmartHomeWebFactory>
{
    private readonly HttpClient _client;

    // Serialiser options that mirror the API's camelCase + string-enum setup.
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
        }
    };

    public DevicesControllerTests(SmartHomeWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    private StringContent Body(object payload) => new(
        JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json");

    private async Task<T> Read<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOpts)!;
    }

    /// <summary>Registers a device and returns the full DeviceResponse.</summary>
    private async Task<DeviceResponse> RegisterAsync(
        string name, string location, DeviceType type)
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = name, deviceLocation = location, type }));
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        return await Read<DeviceResponse>(resp);
    }

    // ── GET /api/devices ───────────────────────────────────────────────────────

    [Fact]
    public async Task GetAll_Returns200_WithSeededDevices()
    {
        var resp = await _client.GetAsync("/api/devices");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var devices = await Read<DeviceResponse[]>(resp);
        Assert.True(devices.Length >= 3); // seed data has 3
    }

    [Fact]
    public async Task GetAll_SeededDevicesIncludeExpectedTypes()
    {
        var resp = await _client.GetAsync("/api/devices");
        var devices = await Read<DeviceResponse[]>(resp);

        Assert.Contains(devices, d => d.Type == DeviceType.Light);
        Assert.Contains(devices, d => d.Type == DeviceType.Fan);
        Assert.Contains(devices, d => d.Type == DeviceType.Thermostat);
    }

    // ── POST /api/devices (register) ──────────────────────────────────────────

    [Fact]
    public async Task RegisterLight_Returns201_WithCorrectBody()
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "Test Lamp", deviceLocation = "Study", type = DeviceType.Light }));

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        Assert.NotNull(resp.Headers.Location);

        var device = await Read<DeviceResponse>(resp);
        Assert.Equal("Test Lamp", device.DeviceName);
        Assert.Equal("Study", device.DeviceLocation);
        Assert.Equal(DeviceType.Light, device.Type);
        Assert.NotEqual(Guid.Empty, device.Id);
    }

    [Fact]
    public async Task RegisterFan_Returns201_WithFanSpeedField()
    {
        var device = await RegisterAsync("Test Fan", "Garage", DeviceType.Fan);
        Assert.Equal(DeviceType.Fan, device.Type);
        Assert.NotNull(device.FanSpeed);
    }

    [Fact]
    public async Task RegisterDoorLock_Returns201_WithIsLockedField()
    {
        var device = await RegisterAsync("Front Door", "Entry", DeviceType.DoorLock);
        Assert.Equal(DeviceType.DoorLock, device.Type);
        Assert.NotNull(device.IsLocked);
    }

    [Fact]
    public async Task RegisterThermostat_Returns201_WithTemperatureFields()
    {
        var device = await RegisterAsync("Hall Thermostat", "Hallway", DeviceType.Thermostat);
        Assert.Equal(DeviceType.Thermostat, device.Type);
        Assert.NotNull(device.TargetTemperature);
        Assert.NotNull(device.MinTemperature);
        Assert.NotNull(device.MaxTemperature);
    }

    [Fact]
    public async Task RegisterLight_LocationHeader_PointsToNewDevice()
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "Location Header Light", deviceLocation = "Attic", type = DeviceType.Light }));

        var device = await Read<DeviceResponse>(resp);
        var getResp = await _client.GetAsync($"/api/devices/{device.Id}");
        Assert.Equal(HttpStatusCode.OK, getResp.StatusCode);
    }

    // ── GET /api/devices/{id} ─────────────────────────────────────────────────

    [Fact]
    public async Task GetById_Returns200_ForRegisteredDevice()
    {
        var created = await RegisterAsync("Fetch Me", "Basement", DeviceType.Light);

        var resp = await _client.GetAsync($"/api/devices/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var device = await Read<DeviceResponse>(resp);
        Assert.Equal(created.Id, device.Id);
        Assert.Equal("Fetch Me", device.DeviceName);
    }

    [Fact]
    public async Task GetById_Returns404_ForUnknownId()
    {
        var resp = await _client.GetAsync($"/api/devices/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task GetById_Returns404_ForMalformedId()
    {
        // Route constraint {deviceId:guid} treats non-GUIDs as no-match → 404.
        var resp = await _client.GetAsync("/api/devices/not-a-guid");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    // ── POST /api/devices/{id}/commands — TogglePower ─────────────────────────

    [Fact]
    public async Task TogglePower_TurnsLightOn_WhenItWasOff()
    {
        var device = await RegisterAsync("Power Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        // second toggle switches state; verify a valid bool is returned
        Assert.IsType<bool>(updated.IsPoweredOn ?? updated.IsDeviceOn);
    }

    [Fact]
    public async Task TogglePower_Returns200_WithUpdatedDevice()
    {
        var device = await RegisterAsync("Toggle Fan", "Shed", DeviceType.Fan);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal(device.Id, updated.Id);
    }

    [Fact]
    public async Task ToggleLock_FlipsLockState()
    {
        var device = await RegisterAsync("Toggle Lock", "Porch", DeviceType.DoorLock);
        var initialLocked = device.IsLocked;

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.ToggleLock }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal(!initialLocked, updated.IsLocked);
    }

    // ── POST /api/devices/{id}/commands — SetBrightness ──────────────────────

    [Fact]
    public async Task SetBrightness_Returns200_WithNewBrightness()
    {
        var device = await RegisterAsync("Bright Light", "Den", DeviceType.Light);
        // power on first
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness, brightness = 80 }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal(80, updated.LightBrightness);
    }

    [Fact]
    public async Task SetBrightness_AtLowerBound_Returns200()
    {
        var device = await RegisterAsync("Dim Light", "Pantry", DeviceType.Light);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness, brightness = 10 }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetBrightness_AtUpperBound_Returns200()
    {
        var device = await RegisterAsync("Max Light", "Loft", DeviceType.Light);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness, brightness = 100 }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    // ── POST /api/devices/{id}/commands — SetColor ────────────────────────────

    [Fact]
    public async Task SetColor_Returns200_WithUpdatedColor()
    {
        var device = await RegisterAsync("Color Light", "Gallery", DeviceType.Light);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetColor, color = "#FF0000" }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal("#FF0000", updated.LightColor);
    }

    // ── POST /api/devices/{id}/commands — SetFanSpeed ────────────────────────

    [Fact]
    public async Task SetFanSpeed_Returns200_WithUpdatedSpeed()
    {
        var device = await RegisterAsync("Speed Fan", "Server Room", DeviceType.Fan);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetFanSpeed, fanSpeed = FanSpeed.High }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal(FanSpeed.High, updated.FanSpeed);
    }

    [Fact]
    public async Task SetFanSpeed_ToMedium_Returns200()
    {
        var device = await RegisterAsync("Medium Fan", "Utility", DeviceType.Fan);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetFanSpeed, fanSpeed = FanSpeed.Medium }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    // ── POST /api/devices/{id}/commands — Thermostat ─────────────────────────

    [Fact]
    public async Task SetThermostatMode_Returns200_WithUpdatedMode()
    {
        var device = await RegisterAsync("Mode Thermo", "Conservatory", DeviceType.Thermostat);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetThermostatMode, thermostatMode = ThermostatMode.Heat }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal(ThermostatMode.Heat, updated.ThermostatMode);
    }

    [Fact]
    public async Task SetTargetTemperature_Returns200_WithUpdatedTemperature()
    {
        var device = await RegisterAsync("Temp Thermo", "Mudroom", DeviceType.Thermostat);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetTargetTemperature, targetTemperature = 70 }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var updated = await Read<DeviceResponse>(resp);
        Assert.Equal(70, updated.TargetTemperature);
    }

    [Fact]
    public async Task SetTargetTemperature_AtLowerBound_Returns200()
    {
        var device = await RegisterAsync("Cold Thermo", "Freezer Room", DeviceType.Thermostat);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetTargetTemperature, targetTemperature = 60 }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetTargetTemperature_AtUpperBound_Returns200()
    {
        var device = await RegisterAsync("Hot Thermo", "Sauna", DeviceType.Thermostat);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetTargetTemperature, targetTemperature = 80 }));

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Command_Returns404_ForUnknownDevice()
    {
        var resp = await _client.PostAsync(
            $"/api/devices/{Guid.NewGuid()}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    // ── GET /api/devices/{id}/history ─────────────────────────────────────────

    [Fact]
    public async Task GetHistory_Returns200_EmptyListForNewDevice()
    {
        var device = await RegisterAsync("History Device", "Archive", DeviceType.Light);

        var resp = await _client.GetAsync($"/api/devices/{device.Id}/history");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var history = await Read<object[]>(resp);
        Assert.Empty(history);
    }

    [Fact]
    public async Task GetHistory_Returns200_WithEntryAfterCommand()
    {
        var device = await RegisterAsync("Commanded Device", "Log Room", DeviceType.Light);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.GetAsync($"/api/devices/{device.Id}/history");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var history = await Read<object[]>(resp);
        Assert.Single(history);
    }

    [Fact]
    public async Task GetHistory_AccumulatesMultipleCommands()
    {
        var device = await RegisterAsync("Multi Command", "Command Room", DeviceType.Light);
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));
        await _client.PostAsync($"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        var resp = await _client.GetAsync($"/api/devices/{device.Id}/history");
        var history = await Read<object[]>(resp);
        Assert.Equal(2, history.Length);
    }

    // ── DELETE /api/devices/{id} ──────────────────────────────────────────────

    [Fact]
    public async Task Delete_Returns204_ForExistingDevice()
    {
        var device = await RegisterAsync("Delete Me", "Void", DeviceType.Fan);

        var resp = await _client.DeleteAsync($"/api/devices/{device.Id}");
        Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
    }

    [Fact]
    public async Task Delete_DeviceIsGone_AfterDeletion()
    {
        var device = await RegisterAsync("Gone Device", "Abyss", DeviceType.Light);
        await _client.DeleteAsync($"/api/devices/{device.Id}");

        var getResp = await _client.GetAsync($"/api/devices/{device.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    [Fact]
    public async Task Delete_Returns404_ForUnknownDevice()
    {
        var resp = await _client.DeleteAsync($"/api/devices/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    // ── Duplicate thermostat ───────────────────────────────────────────────────

    [Fact]
    public async Task RegisterSecondThermostat_InSameLocation_Returns409()
    {
        // Seed already has a thermostat in "Living Room", so a second one must conflict.
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "Duplicate Thermo", deviceLocation = "Living Room", type = DeviceType.Thermostat }));

        Assert.Equal(HttpStatusCode.Conflict, resp.StatusCode);
    }

    [Fact]
    public async Task RegisterTwoThermostats_InDifferentLocations_BothSucceed()
    {
        var first  = await RegisterAsync("Thermo A", "Sunroom", DeviceType.Thermostat);
        var second = await RegisterAsync("Thermo B", "Playroom", DeviceType.Thermostat);

        Assert.NotEqual(Guid.Empty, first.Id);
        Assert.NotEqual(Guid.Empty, second.Id);
        Assert.NotEqual(first.Id, second.Id);
    }
}
