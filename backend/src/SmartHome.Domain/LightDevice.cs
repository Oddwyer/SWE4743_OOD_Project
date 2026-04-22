using SmartHome.Domain.Interfaces;

namespace SmartHome.Domain;

public class LightDevice : Device, IPoweredDevice, ILightColor, IDimLights{}