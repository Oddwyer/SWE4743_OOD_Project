using System.ComponentModel.DataAnnotations;

namespace SmartHome.Api.Simulations;

/// <summary>
/// DTO for setting the ambient temperature of a location.
/// </summary>

public class SetAmbientTemperatureRequest
{
    // Input Validation at API Layer
    [Required]
    [Range(0, 120)]
    public int Temperature { get; set; }

}