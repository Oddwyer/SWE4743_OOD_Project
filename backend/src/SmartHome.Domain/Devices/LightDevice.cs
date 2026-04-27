//using System.Reflection.Metadata.Ecma335;

namespace SmartHome.Domain.Devices;

public class LightDevice : Device, IPoweredDevice, ILightColor, IDimLights
{
    private DevicePowerState _powerState;
    private LightColorState _colorState;
    private int _brightness;

    public LightDevice(Guid id, string deviceName, string deviceLocation) : base(id, deviceName, deviceLocation, DeviceType.Light)
    {
        _powerState = DevicePowerState.Off; // default state
        _colorState = LightColorState.White; // default color
        _brightness = 10; // default brightness
    }

    public DevicePowerState powerState => _powerState;

    public void TogglePower()
    {
        // trying toggle with a ternary operator for cleaner code
        
        _powerState = _powerState == DevicePowerState.On // check current state and toggle
        ? DevicePowerState.Off  // if on, turn off
        : DevicePowerState.On;  // if off, turn on
    }

    public override bool IsDeviceOn => _powerState == DevicePowerState.On;

    public LightColorState colorState => _colorState;

    public void ChangeColor(LightColorState newColor)
    {
        _colorState = newColor;
    }

    public int lightBrightness => _brightness;

    public void setLightBrightness(int brightnessPercentage)
    {
        if (brightnessPercentage < 10 || brightnessPercentage > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(brightnessPercentage), "Brightness must be between 10 and 100.");
        }
        _brightness = brightnessPercentage;
    }
}