using SmartHome.Domain.Devices.Light.LightStates;

namespace SmartHome.Domain.Devices.Light;

public class LightDevice : Device, IPoweredDevice, ILightColor, IDimLights
{
    // States
    private DevicePowerState _powerState;

    public OffState Off { get; private set; }

    private ILightState _currentState;

    public LightColor ColorState { get; private set; }

    public int LightBrightness { get; private set; }

    public LightDevice(Guid id, string deviceName, string deviceLocation) : base(id, deviceName, deviceLocation, DeviceType.Light)
    {
        _powerState = DevicePowerState.Off; // default state
        Off = new OffState(this);
        _currentState = Off; // default state
        ColorState = LightColor.White; // default color
        LightBrightness = 10; // default brightness
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

    }

    /// <summary>
    /// Sets the power state to off (used by states).
    /// </summary>
    internal void TurnPowerOff()
    {
        _powerState = DevicePowerState.Off;

    }

    /// <summary>
    /// Requests a color change. The current state decides if it is allowed.
    /// </summary>
    public void ChangeColor(LightColor newColor)
    {
        _currentState.ChangeColor(newColor);
    }

    /// <summary>
    /// Applies a color change (used by states).
    /// </summary>
    internal void ChangeColorInternal(LightColor newColor)
    {
        ColorState = newColor;
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
    }

    /// <summary>
    /// Updates the status message (used by states). The status message can be used for logging, debugging, or providing user feedback through the API.
    /// </summary>
    internal void UpdateStatusMessage(string message)
    {
        StatusMessage = message;
    }
}
