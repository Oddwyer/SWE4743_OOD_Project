namespace SmartHome.Api.Locations;

/// <summary>
/// DTO for setting the ambient temperature of a location.
/// </summary>

public class SetAmbientTemperatureRequest
{

    public int Temperature { get; set; }

}