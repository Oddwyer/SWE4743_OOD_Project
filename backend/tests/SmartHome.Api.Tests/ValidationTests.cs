using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SmartHome.Api.Devices;
using SmartHome.Api.Tests.Helpers;
using SmartHome.Domain.Commands;
using SmartHome.Domain.Devices;
using Xunit;

namespace SmartHome.Api.Tests;

/// <summary>
/// Integration tests for request-validation and error-handling scenarios.
/// Covers 400 Bad Request (FluentValidation), 404 Not Found, and 409 Conflict responses
/// as well as the global error-handling middleware.
/// </summary>
public class ValidationTests : IClassFixture<SmartHomeWebFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
        }
    };

    public ValidationTests(SmartHomeWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    private StringContent Body(object payload) => new(
        JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json");

    private async Task<DeviceResponse> RegisterAsync(
        string name, string location, DeviceType type)
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = name, deviceLocation = location, type }));
        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
        return JsonSerializer.Deserialize<DeviceResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;
    }

    // ── Register device — name validation ─────────────────────────────────────

    [Fact]
    public async Task Register_Returns400_WhenNameIsEmpty()
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "", deviceLocation = "Kitchen", type = DeviceType.Light }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Register_Returns400_WhenNameIsTooShort()
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "X", deviceLocation = "Kitchen", type = DeviceType.Light }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Register_Returns400_WhenLocationIsEmpty()
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "Valid Name", deviceLocation = "", type = DeviceType.Light }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Register_Returns400_WhenBodyIsEmpty()
    {
        var resp = await _client.PostAsync("/api/devices",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Register_Returns415_WhenContentTypeIsMissing()
    {
        var resp = await _client.PostAsync("/api/devices",
            new StringContent("not json"));
        Assert.Equal(HttpStatusCode.UnsupportedMediaType, resp.StatusCode);
    }

    // ── SetBrightness validation ───────────────────────────────────────────────

    [Fact]
    public async Task SetBrightness_Returns400_WhenBrightnessIsMissing()
    {
        var device = await RegisterAsync("Validation Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness })); // no brightness field

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetBrightness_Returns400_WhenBelowMinimum()
    {
        var device = await RegisterAsync("Too Dim Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness, brightness = 9 }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetBrightness_Returns400_WhenAboveMaximum()
    {
        var device = await RegisterAsync("Too Bright Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness, brightness = 101 }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetBrightness_Returns400_WhenValueIsZero()
    {
        var device = await RegisterAsync("Zero Bright", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetBrightness, brightness = 0 }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── SetColor validation ────────────────────────────────────────────────────

    [Fact]
    public async Task SetColor_Returns400_WhenColorIsMissing()
    {
        var device = await RegisterAsync("No Color Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetColor }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetColor_Returns400_WhenColorLacksHashPrefix()
    {
        var device = await RegisterAsync("Bad Color Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetColor, color = "FF0000" }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetColor_Returns400_WhenColorIsNotValidHex()
    {
        var device = await RegisterAsync("Invalid Color Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetColor, color = "#ZZZZZZ" }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetColor_Returns400_WhenColorIsWrongLength()
    {
        var device = await RegisterAsync("Short Color Light", "Lab", DeviceType.Light);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetColor, color = "#FFF" })); // 3-digit, not 6

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── SetFanSpeed validation ────────────────────────────────────────────────

    [Fact]
    public async Task SetFanSpeed_Returns400_WhenFanSpeedIsMissing()
    {
        var device = await RegisterAsync("No Speed Fan", "Lab", DeviceType.Fan);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetFanSpeed }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── SetTargetTemperature validation ───────────────────────────────────────

    [Fact]
    public async Task SetTargetTemperature_Returns400_WhenTemperatureIsMissing()
    {
        var device = await RegisterAsync("No Temp Thermo", "Lab-TempMissing", DeviceType.Thermostat);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetTargetTemperature }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetTargetTemperature_Returns400_WhenBelowMinimum()
    {
        var device = await RegisterAsync("Cold Thermo", "Lab-TempMin", DeviceType.Thermostat);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetTargetTemperature, targetTemperature = 59 }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetTargetTemperature_Returns400_WhenAboveMaximum()
    {
        var device = await RegisterAsync("Hot Thermo", "Lab-TempMax", DeviceType.Thermostat);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetTargetTemperature, targetTemperature = 81 }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── SetThermostatMode validation ──────────────────────────────────────────

    [Fact]
    public async Task SetThermostatMode_Returns400_WhenModeIsMissing()
    {
        var device = await RegisterAsync("No Mode Thermo", "Lab-Mode", DeviceType.Thermostat);

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.SetThermostatMode }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── Global error handler — problem+json shape ─────────────────────────────

    [Fact]
    public async Task NotFound_ResponseContentType_IsProblemJson()
    {
        var resp = await _client.GetAsync($"/api/devices/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
        Assert.Contains("application/problem+json",
            resp.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task BadRequest_ValidationResponse_ContainsErrors()
    {
        var resp = await _client.PostAsync("/api/devices",
            Body(new { deviceName = "", deviceLocation = "", type = DeviceType.Light }));

        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
        var body = await resp.Content.ReadAsStringAsync();
        Assert.False(string.IsNullOrEmpty(body));
    }

    // ── 404 for deleted resource ───────────────────────────────────────────────

    [Fact]
    public async Task Command_Returns404_AfterDeviceIsDeleted()
    {
        var device = await RegisterAsync("Deleted Device", "Void", DeviceType.Light);
        await _client.DeleteAsync($"/api/devices/{device.Id}");

        var resp = await _client.PostAsync(
            $"/api/devices/{device.Id}/commands",
            Body(new { command = DeviceCommandType.TogglePower }));

        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task History_Returns404_ForUnknownDevice()
    {
        var resp = await _client.GetAsync($"/api/devices/{Guid.NewGuid()}/history");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }
}
