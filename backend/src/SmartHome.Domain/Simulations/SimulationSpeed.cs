using System.ComponentModel;

namespace SmartHome.Domain.Simulations;

/// <summary>
/// Speed multipliers controlling how fast the ambient temperature simulation advances.
/// </summary>
public enum SimulationSpeed
{
    /// <summary>Normal speed — one simulation tick every 5 seconds.</summary>
    [Description("1x")] OneX = 1,
    /// <summary>2× speed — one simulation tick every 2.5 seconds.</summary>
    [Description("2x")] TwoX = 2,
    /// <summary>5× speed — one simulation tick every 1 second.</summary>
    [Description("5x")] FiveX = 5,
    /// <summary>10× speed — one simulation tick every 0.5 seconds.</summary>
    [Description("10x")] TenX = 10
}
