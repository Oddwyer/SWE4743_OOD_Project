using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SmartHome.Api.Tests;

/// <summary>
/// Integration tests for SimulationController and LocationsController.
/// Covers POST /api/simulation/reset, PUT /api/simulation/speed,
/// GET /api/locations/{location}/ambient-temperature, and
/// PUT /api/locations/{location}/ambient-temperature.
/// </summary>
public class SimulationControllerTests : IClassFixture<SmartHomeWebFactory>
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

    public SimulationControllerTests(SmartHomeWebFactory factory)
    {
        _client = factory.CreateClient();
    }

    private StringContent Body(object payload) => new(
        JsonSerializer.Serialize(payload, JsonOpts), Encoding.UTF8, "application/json");

    // ── POST /api/simulation/reset ────────────────────────────────────────────

    [Fact]
    public async Task Reset_Returns200()
    {
        var resp = await _client.PostAsync("/api/simulation/reset", null);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task Reset_ReturnsSuccessMessage()
    {
        var resp = await _client.PostAsync("/api/simulation/reset", null);
        var body = JsonSerializer.Deserialize<SimulationResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;

        Assert.False(string.IsNullOrEmpty(body.Message));
    }

    // ── PUT /api/simulation/speed ─────────────────────────────────────────────

    [Fact]
    public async Task SetSpeed_Returns200_ForOneX()
    {
        var resp = await _client.PutAsync("/api/simulation/speed",
            Body(new { speedMultiplier = SimulationSpeed.OneX }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetSpeed_Returns200_ForTwoX()
    {
        var resp = await _client.PutAsync("/api/simulation/speed",
            Body(new { speedMultiplier = SimulationSpeed.TwoX }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetSpeed_Returns200_ForFiveX()
    {
        var resp = await _client.PutAsync("/api/simulation/speed",
            Body(new { speedMultiplier = SimulationSpeed.FiveX }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetSpeed_Returns200_ForTenX()
    {
        var resp = await _client.PutAsync("/api/simulation/speed",
            Body(new { speedMultiplier = SimulationSpeed.TenX }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetSpeed_ReturnsSpeedMultiplierInResponse()
    {
        var resp = await _client.PutAsync("/api/simulation/speed",
            Body(new { speedMultiplier = SimulationSpeed.TwoX }));

        var body = JsonSerializer.Deserialize<SimulationResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;

        Assert.Equal(SimulationSpeed.TwoX, body.SpeedMultiplier);
    }

    [Fact]
    public async Task SetSpeed_Returns400_ForInvalidSpeed()
    {
        var json = """{"speedMultiplier": "invalidSpeed"}""";
        var resp = await _client.PutAsync("/api/simulation/speed",
            new StringContent(json, Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetSpeed_Returns400_WhenBodyIsEmpty()
    {
        var resp = await _client.PutAsync("/api/simulation/speed",
            new StringContent("{}", Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    // ── GET /api/locations/{location}/ambient-temperature ─────────────────────

    [Fact]
    public async Task GetAmbientTemperature_Returns200_ForSeededLocation()
    {
        var resp = await _client.GetAsync("/api/locations/Living Room/ambient-temperature");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task GetAmbientTemperature_ReturnsExpectedFields_ForSeededLocation()
    {
        var resp = await _client.GetAsync("/api/locations/Living Room/ambient-temperature");
        var body = JsonSerializer.Deserialize<AmbientTemperatureResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;

        Assert.Equal("Living Room", body.Location);
        Assert.Equal(0, body.MinTemperature);
        Assert.Equal(100, body.MaxTemperature);
        Assert.Equal(72, body.DefaultTemperature);
        Assert.InRange(body.AmbientTemperature, 0, 100);
    }

    [Fact]
    public async Task GetAmbientTemperature_Returns200_ForUnknownLocation()
    {
        // Unknown locations return the default temperature — no 404.
        var resp = await _client.GetAsync("/api/locations/Attic/ambient-temperature");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task GetAmbientTemperature_ReturnsDefaultTemperature_ForUnknownLocation()
    {
        var resp = await _client.GetAsync("/api/locations/Attic/ambient-temperature");
        var body = JsonSerializer.Deserialize<AmbientTemperatureResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;

        Assert.Equal(72, body.AmbientTemperature);
    }

    // ── PUT /api/locations/{location}/ambient-temperature ─────────────────────

    [Fact]
    public async Task SetAmbientTemperature_Returns200_WithValidTemperature()
    {
        var resp = await _client.PutAsync("/api/locations/Kitchen/ambient-temperature",
            Body(new { temperature = 75 }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetAmbientTemperature_ReturnsUpdatedTemperature()
    {
        var resp = await _client.PutAsync("/api/locations/Kitchen/ambient-temperature",
            Body(new { temperature = 80 }));

        var body = JsonSerializer.Deserialize<AmbientTemperatureResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;

        Assert.Equal("Kitchen", body.Location);
        Assert.Equal(80, body.AmbientTemperature);
    }

    [Fact]
    public async Task SetAmbientTemperature_Returns200_AtMinimum()
    {
        var resp = await _client.PutAsync("/api/locations/Bedroom/ambient-temperature",
            Body(new { temperature = 0 }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetAmbientTemperature_Returns200_AtMaximum()
    {
        var resp = await _client.PutAsync("/api/locations/Bedroom/ambient-temperature",
            Body(new { temperature = 100 }));
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task SetAmbientTemperature_Returns400_WhenBelowMinimum()
    {
        var resp = await _client.PutAsync("/api/locations/Kitchen/ambient-temperature",
            Body(new { temperature = -1 }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetAmbientTemperature_Returns400_WhenAboveMaximum()
    {
        var resp = await _client.PutAsync("/api/locations/Kitchen/ambient-temperature",
            Body(new { temperature = 101 }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetAmbientTemperature_Returns400_WhenTemperatureIsNegative()
    {
        var resp = await _client.PutAsync("/api/locations/Kitchen/ambient-temperature",
            Body(new { temperature = -5 }));
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task SetAmbientTemperature_PersistsAcrossGet()
    {
        await _client.PutAsync("/api/locations/Living Room/ambient-temperature",
            Body(new { temperature = 55 }));

        var resp = await _client.GetAsync("/api/locations/Living Room/ambient-temperature");
        var body = JsonSerializer.Deserialize<AmbientTemperatureResponse>(
            await resp.Content.ReadAsStringAsync(), JsonOpts)!;

        Assert.Equal(55, body.AmbientTemperature);
    }
}
