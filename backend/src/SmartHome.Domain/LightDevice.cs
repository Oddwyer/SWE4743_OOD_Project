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

    int lightBrightness { get; } // can be used to check current brightness level and for rehydration purposes
    void setLightBrightness(int brightnessPercentage) // brightness level from 0 to 100
    {
        lightBrightness = brightnessPercentage;
    } 


    void TogglePower()
    {
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