using SmartHome.Domain.Devices.Light.LightStates;
using SmartHome.Domain.Commands.Light;

namespace SmartHome.Domain.Devices.Light;

public class LightDevice : Device, IPoweredDevice, ILightColor, IDimLights
{
    public int MinBrightness => 10; // Minimum allowed brightness percentage
    public int MaxBrightness => 100; // Maximum allowed brightness percentage

    // States
    private DevicePowerState _powerState = DevicePowerState.Off; // Default state
    public OffState Off { get; private set; }
    public OnState On { get; private set; }

    private ILightState _currentState;

    public string Color { get; private set; } = "#FFFFFF"; // Default color (white)

    public int LightBrightness { get; private set; } = 100; // Default brightness (100%)


    public LightDevice(Guid id, string deviceName, string deviceLocation) : base(id, deviceName, deviceLocation, DeviceType.Light, new LightCommandFactory())
    {

        Off = new OffState(this);
        On = new OnState(this);
        _currentState = Off; // default state
    }

    /// <summary>
    /// Current power state of the light.
    /// </summary>
    public DevicePowerState PowerState => _powerState;

    /// <summary>
    /// Indicates whether the light is on.
    /// </summary>
    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    /// <summary>
    /// Requests a power toggle. Behavior is determined by the current state.
    /// </summary>
    public void TogglePower()
    {
        _currentState.TogglePower();
    }

    /// <summary>
    /// Sets the power state to on (used by states).
    /// </summary>
    internal void TurnPowerOn()
    {
        _powerState = DevicePowerState.On;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Sets the power state to off (used by states).
    /// </summary>
    internal void TurnPowerOff()
    {
        _powerState = DevicePowerState.Off;
        UpdatedAt = DateTime.UtcNow;

    }

    /// <summary>
    /// Requests a color change. The current state decides if it is allowed.
    /// </summary>
    public void SetColor(string newColor)
    {
        _currentState.ChangeColor(newColor);
    }

    /// <summary>
    /// Applies a color change (used by states).
    /// </summary>
    internal void SetColorInternal(string newColor)
    {
        Color = newColor;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Requests a brightness change. The current state decides if it is allowed.
    /// </summary>
    public void SetLightBrightness(int brightnessPercentage)
    {
        _currentState.SetLightBrightness(brightnessPercentage);
    }

    /// <summary>
    /// Applies a brightness change (used by states).
    /// </summary>
    internal void SetLightBrightnessInternal(int brightnessPercentage)
    {
        LightBrightness = brightnessPercentage;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the current state (used by states).`
    /// </summary>
    internal void SetState(ILightState newState)
    {
        _currentState = newState;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Restores device properties.
    /// </summary>
    internal void RehydrateState(DevicePowerState powerState, string color, int brightness)
    {
        _powerState = powerState;
        Color = color;
        LightBrightness = brightness;
        _currentState = powerState == DevicePowerState.On ? On : Off;
    }

    /// <summary>
    /// Resets device properties to default settings.
    /// </summary>
    public override void ResetToDefault()
    {
        _powerState = DevicePowerState.Off;
        _currentState = Off;
        SetColorInternal("#FFFFFF");
        SetLightBrightnessInternal(100);
        UpdatedAt = DateTime.UtcNow;

    }


}
