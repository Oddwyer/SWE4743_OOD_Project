/** Commands that can be sent to a device via the API. */
export type DeviceCommandType =
  | 'TogglePower'
  | 'SetBrightness'
  | 'SetColor'
  | 'SetFanSpeed'
  | 'SetThermostatMode'
  | 'SetTargetTemperature'
  | 'ToggleLock';
