using System.ComponentModel;

namespace SmartHome.Domain.Simulations;

public enum SimulationSpeed
{
    [Description("1x")] OneX = 1,
    [Description("2x")] TwoX = 2,
    [Description("5x")] FiveX = 5,
    [Description("10x")] TenX = 10
}
