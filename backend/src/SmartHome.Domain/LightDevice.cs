//using System.Reflection.Metadata.Ecma335;
using SmartHome.Domain.Interfaces;

namespace SmartHome.Domain;

public class LightDevice : IDevice, IPoweredDevice, ILightColor, IDimLights
{
    //TODO: Replace with Kataali's code. 
    public Guid Id { get; }
    public string DeviceName { get; }
    public string DeviceLocation { get; }
    public DeviceType Type { get; }

    //public DeviceState State {get;}
    DevicePowerState powerState { get; }
    public bool IsDeviceOn { get; }
    LightColorState colorState { get; }

    void TogglePower(){
        if (powerState.On)
        {
            powerState = powerState.Off;
        }
        else
        {
            powerState = powerState.On;
        }
    }
    void ChangeColor(LightColorState newColor)
    {
        colorState = newColor;
    }

}