using System.ComponentModel;

namespace SmartHome.Domain.Simulations;

public enum SimulationSpeed
{
    [Description("1x")] x1 = 1,
    [Description("2x")] x2 = 2,
    [Description("5x")] x5 = 5,
    [Description("10x")] x10 = 10
}